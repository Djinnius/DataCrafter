using CsvHelper;
using System.Globalization;
using DataCrafter.Services.ConsoleWriters;
using Spectre.Console.Cli;
using Spectre.Console;
using FluentValidation;
using DataCrafter.Entities;

namespace DataCrafter.Commands.DataFrame.PlotCsvColumn;
internal sealed class PlotCsvColumnCommand : Command<PlotCsvColumnCommandSettings>
{
    private readonly IAnsiConsole _ansiConsole;
    private readonly IDistributionPlotterConsoleWriter _distributionPlotterConsoleWriter;
    private readonly IValidator<PlotCsvColumnCommandSettings> _validator;

    public PlotCsvColumnCommand(
        IAnsiConsole ansiConsole,
        IDistributionPlotterConsoleWriter distributionPlotterConsoleWriter,
        IValidator<PlotCsvColumnCommandSettings> validator)
    {
        _ansiConsole = ansiConsole;
        _distributionPlotterConsoleWriter = distributionPlotterConsoleWriter;
        _validator = validator;
    }

    public override int Execute(CommandContext context, PlotCsvColumnCommandSettings settings)
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

        var numberOfBuckets = settings.Buckets.IsSet ? settings.Buckets.Value : 20;
        _distributionPlotterConsoleWriter.PlotHistogram(columnStatistics!, numberOfBuckets, settings.Name);

        return 0;
    }

    private int ValidateSettings(PlotCsvColumnCommandSettings settings)
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
