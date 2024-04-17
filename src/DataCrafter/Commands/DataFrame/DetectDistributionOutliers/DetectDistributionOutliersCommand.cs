using Accord.Statistics.Distributions;
using CsvHelper;
using DataCrafter.Entities;
using DataCrafter.Services.ConsoleWriters;
using DataCrafter.Services.Distributions;
using FluentValidation;
using Spectre.Console;
using Spectre.Console.Cli;
using System.Globalization;

namespace DataCrafter.Commands.DataFrame.DetectDistributionOutliers;
internal class DetectDistributionOutliersCommand : Command<DetectDistributionOutliersCommandSettings>
{
    private readonly IAnsiConsole _ansiConsole;
    private readonly IValidator<DetectDistributionOutliersCommandSettings> _validator;
    private readonly IDistributionProvider _distributionProvider;
    private readonly IDistributionInfoService _distributionInfoService;
    private readonly IDistributionPlotterConsoleWriter _distributionPlotterConsoleWriter;

    public DetectDistributionOutliersCommand(
        IAnsiConsole ansiConsole,
        IValidator<DetectDistributionOutliersCommandSettings> validator,
        IDistributionProvider distributionProvider,
        IDistributionInfoService distributionInfoService,
        IDistributionPlotterConsoleWriter distributionPlotterConsoleWriter)
    {
        _ansiConsole = ansiConsole;
        _validator = validator;
        _distributionProvider = distributionProvider;
        _distributionInfoService = distributionInfoService;
        _distributionPlotterConsoleWriter = distributionPlotterConsoleWriter;
    }

    public override int Execute(CommandContext context, DetectDistributionOutliersCommandSettings settings)
    {
        if (ValidateSettings(settings) < 0)
            return -1;

        var inputFilePath = settings.Input.IsSet ? settings.Input.Value : "data.csv";

        if (!inputFilePath.EndsWith(".csv", StringComparison.OrdinalIgnoreCase))
        {
            _ansiConsole.MarkupLine($"[yellow]Warning:[/] Input file '{inputFilePath}' is not a CSV file.");
            return -1;
        }

        if (!File.Exists(inputFilePath))
        {
            _ansiConsole.MarkupLine($"[yellow]Warning:[/] Input file '{inputFilePath}' does not exist.");
            return -1;
        }

        var statistics = CalculateStatistics(inputFilePath);

        if (!statistics.TryGetValue(settings.Name, out var columnStatistics))
        {
            _ansiConsole.MarkupLine($"[red]Error:[/] No column by the name {settings.Name} exists.");
            _ansiConsole.MarkupLine($"The following columns {string.Join(",", statistics.Keys.Select(x => $"[bold yellow]{x}[/]"))} are valid.");
            return -1;
        }

        var univariateDistributions = _distributionProvider.GetUnivariateDistributions();
        var distributionName = settings.Distribution.ToLower();

        // Find all distributions that match the given name and prioritise them based on text position
        var matchedDistributions = univariateDistributions
            .Select(d => new
            {
                Distribution = d,
                MatchIndex = _distributionInfoService.GetDistributionProperties(d).Name.ToLower().IndexOf(distributionName)
            })
            .Where(match => match.MatchIndex != -1)
            .OrderBy(match => match.MatchIndex)
            .Select(match => match.Distribution)
            .ToList();

        // Get the first matched distribution (prioritised by text position)
        var distribution = matchedDistributions.FirstOrDefault();

        if (distribution == null)
        {
            _ansiConsole.MarkupLine($"[red]Error:[/] Distribution '{settings.Distribution}' not found or not supported.");
            return -1;
        }

        if (distribution is not IFittable<double> fittableDistribution)
        {
            _ansiConsole.MarkupLine($"[red]Error:[/] The chosen distribution {_distributionInfoService.GetDistributionProperties(distribution).Name} based on the name '{settings.Distribution}' is not fittable.");
            return -1;
        }

        _ansiConsole.MarkupLine($"The chosen distribution is {_distributionInfoService.GetDistributionProperties(distribution).Name} based on the name '{settings.Distribution}'.");

        // Fit the distribution to the data
        var data = columnStatistics.ValuesArray;
        var mean = columnStatistics.Mean;
        fittableDistribution.Fit(data);

        // Detect outliers based on a threshold
        var outliers = new List<double>();
        var threshold = settings.Threshold.IsSet ? settings.Threshold.Value : 0.05;

        for (int i = 0; i < data.Length; i++)
        {
            double pdf = distribution.ProbabilityFunction(data[i]);
            if (pdf < threshold)
            {
                outliers.Add(data[i]);
            }
        }

        _ansiConsole.MarkupLine($"With the p-value threshold at {threshold}, {outliers.Count} were detected from {columnStatistics.Values.Count}.");

        // Report outliers in a table
        WriteOutliersToConsole(outliers, mean);

        var outliersBelowMean = outliers.Where(x => x < mean).ToList();
        var outliersAboveMean = outliers.Where(x => x > mean).ToList();

        // Get the max below the mean and the min above the mean
        double maxBelowMean = outliersBelowMean.Any() ? outliersBelowMean.Max() : double.NaN;
        double minAboveMean = outliersAboveMean.Any() ? outliersAboveMean.Min() : double.NaN;


        _distributionPlotterConsoleWriter.PlotHistogramWithOutliers(columnStatistics, maxBelowMean, minAboveMean, 40, distributionName);

        return 0;
    }

    private void WriteOutliersToConsole(IEnumerable<double> outliers, double mean)
    {
        var outlierTable = new Table()
            .LeftAligned()
            .Title("Outliers")
            .AddColumn("Values");

        foreach(var outlier in outliers.OrderBy(x => x))
        {
            var color = outlier < mean ? Color.LightSkyBlue1 : Color.LightPink1;
            outlierTable.AddRow(new Markup($"[{color}]{outlier}[/]"));
        }

        _ansiConsole.Write(outlierTable);
    }

    private int ValidateSettings(DetectDistributionOutliersCommandSettings settings)
    {
        var validationResult = _validator.Validate(settings);

        if (!validationResult.IsValid)
        {
            foreach (var error in validationResult.Errors)
                _ansiConsole.MarkupLine($"[red]Validation Error: {error.ErrorMessage}[/]");

            return -1;
        }

        return 0;
    }

    private Dictionary<string, DataColumn> CalculateStatistics(string filePath)
    {
        var columnStatistics = new Dictionary<string, DataColumn>();

        using (var reader = new StreamReader(filePath))
        using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
        {
            csv.Read();
            csv.ReadHeader();
            var headers = csv.HeaderRecord;

            while (csv.Read())
            {
                foreach (var header in headers!)
                {
                    if (!columnStatistics.ContainsKey(header))
                        columnStatistics[header] = new DataColumn();

                    var value = csv.GetField<double>(header);
                    columnStatistics[header].Add(value);
                }
            }
        }

        return columnStatistics;
    }
}
