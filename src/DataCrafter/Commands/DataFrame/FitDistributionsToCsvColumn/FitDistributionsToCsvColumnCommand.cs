using CsvHelper;
using System.Globalization;
using Spectre.Console;
using Spectre.Console.Cli;
using FluentValidation;
using Accord.Statistics.Analysis;
using DataCrafter.Services.Distributions;
using Accord.Statistics.Distributions;
using DataCrafter.Entities;

namespace DataCrafter.Commands.DataFrame.FitDistributionsToCsvColumn;
internal sealed class FitDistributionsToCsvColumnCommand : Command<FitDistributionsToCsvColumnCommandSettings>
{
    private readonly IAnsiConsole _ansiConsole;
    private readonly IValidator<FitDistributionsToCsvColumnCommandSettings> _validator;
    private readonly IDistributionProvider _distributionProvider;

    public FitDistributionsToCsvColumnCommand(
        IAnsiConsole ansiConsole,
        IValidator<FitDistributionsToCsvColumnCommandSettings> validator,
        IDistributionProvider distributionProvider)
    {
        _ansiConsole = ansiConsole;
        _validator = validator;
        _distributionProvider = distributionProvider;
    }

    public override int Execute(CommandContext context, FitDistributionsToCsvColumnCommandSettings settings)
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

        var da = new DistributionAnalysis();
        var univariateDistributions = _distributionProvider.GetUnivariateDistributions();

        foreach (var distribution in univariateDistributions)
        {
            if (distribution is IFittableDistribution<double> fittableDistribution && da.Distributions.All(x => x.ToString() != fittableDistribution.ToString()))
                da.Distributions.Add(fittableDistribution);
        }

        var fit = da.Learn(columnStatistics.ValuesArray);

        AnsiConsole.WriteLine();
        AnsiConsole.WriteLine("Results");

        var resultTable = new Table().Title($"{settings.Name} Fitting Ranks");

        resultTable.AddColumn("Name");
        resultTable.AddColumn("ChiSquare");
        resultTable.AddColumn("ChiSquareRank");
        resultTable.AddColumn("AndersonDarling");
        resultTable.AddColumn("AndersonDarlingRank");
        resultTable.AddColumn("KolmogorovSmirnov");
        resultTable.AddColumn("KolmogorovSmirnovRank");

        foreach (var distribution in fit)
        {
            resultTable.AddRow(
                distribution.Name,
                distribution.ChiSquare.ToString("N2"),
                distribution.ChiSquareRank.ToString(),
                distribution.AndersonDarling.ToString("N2"),
                distribution.AndersonDarlingRank.ToString(),
                distribution.KolmogorovSmirnov.ToString("N2"),
                distribution.KolmogorovSmirnovRank.ToString()
            );
        }

        _ansiConsole.Write(resultTable);
        return 0;
    }

    private int ValidateSettings(FitDistributionsToCsvColumnCommandSettings settings)
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

    private Dictionary<string, ColumnStatistics> CalculateStatistics(string filePath)
    {
        var columnStatistics = new Dictionary<string, ColumnStatistics>();

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
                        columnStatistics[header] = new ColumnStatistics();

                    var value = csv.GetField<double>(header);
                    columnStatistics[header].Add(value);
                }
            }
        }

        return columnStatistics;
    }
}
