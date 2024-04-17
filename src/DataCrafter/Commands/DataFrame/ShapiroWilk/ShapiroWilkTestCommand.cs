using Accord.Statistics.Testing;
using CsvHelper;
using DataCrafter.Entities;
using FluentValidation;
using Spectre.Console;
using Spectre.Console.Cli;
using System.Globalization;
using System.Text;

namespace DataCrafter.Commands.DataFrame.ShapiroWilk;
internal sealed class ShapiroWilkTestCommand : Command<ShapiroWilkTestCommandSettings>
{
    private readonly IAnsiConsole _ansiConsole;
    private readonly IValidator<ShapiroWilkTestCommandSettings> _validator;

    public ShapiroWilkTestCommand(
        IAnsiConsole ansiConsole,
        IValidator<ShapiroWilkTestCommandSettings> validator)
    {
        _ansiConsole = ansiConsole;
        _validator = validator;
    }

    public override int Execute(CommandContext context, ShapiroWilkTestCommandSettings settings)
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

        _ansiConsole.MarkupLine($"Number of Rows: [yellow]{columnStatistics.Values.Count}[/]");

        // Perform Shapiro-Wilk test
        var shapiroWilkTest = new ShapiroWilkTest(columnStatistics.ValuesArray);

        // Print the result
        _ansiConsole.MarkupLine($"Test statistic: [yellow]{shapiroWilkTest.Statistic.ToString("F3")}[/]");
        _ansiConsole.MarkupLine($"p-value: [yellow]{shapiroWilkTest.PValue.ToString("F3")}[/]");
        _ansiConsole.MarkupLine($"Size: [yellow]{shapiroWilkTest.Size.ToString("F3")}[/]");
        _ansiConsole.MarkupLine($"Null hypothesis (H0) rejected: [yellow]{shapiroWilkTest.Significant}[/]");

        // Interpret the result
        if (shapiroWilkTest.Significant)
            _ansiConsole.MarkupLine("[darkorange3_1]The data is not normally distributed.[/]");
        else
        {
            _ansiConsole.MarkupLine("[green]The data appears to be normally distributed.[/]");

            var lowerBound = columnStatistics.Mean - (2 * columnStatistics.StandardDeviation);
            var upperBound = columnStatistics.Mean + (2 * columnStatistics.StandardDeviation);
            DetectOutliers(inputFilePath, settings.Name, lowerBound, upperBound);
        }

        return 0;
    }

    private void DetectOutliers(string filePath, string header, double lowerBound, double upperBound)
    {
        using var reader = new StreamReader(filePath);
        using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);

        csv.Read();
        csv.ReadHeader();
        var headers = csv.HeaderRecord;

        if (headers is null)
        {
            _ansiConsole.MarkupLine("[red]No headers detected.[/]");
            return;
        }

        while (csv.Read())
        {

            var fieldValue = csv.GetField(header);
            if (!string.IsNullOrWhiteSpace(fieldValue) &&
                double.TryParse(fieldValue, out double value) &&
                (value < lowerBound || value > upperBound))
            {
                var record = new StringBuilder();

                foreach (var h in headers)
                {
                    record.Append(csv.GetField(h));
                    record.Append(",");
                }
                // Remove the trailing comma
                record.Length--;

                _ansiConsole.WriteLine(record.ToString());
            }
        }
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
                    var fieldValue = csv.GetField(header);
                    if (!string.IsNullOrWhiteSpace(fieldValue))
                    {
                        if (!columnStatistics.ContainsKey(header))
                            columnStatistics[header] = new DataColumn();

                        if (double.TryParse(fieldValue, out double value))
                        {
                            if (value == 0)
                                System.Diagnostics.Debugger.Break();

                            columnStatistics[header].Add(value);
                        }
                    }
                }
            }
        }

        return columnStatistics;
    }

    private int ValidateSettings(ShapiroWilkTestCommandSettings settings)
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

}
