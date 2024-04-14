using CsvHelper;
using DataCrafter.Entities;
using DataCrafter.Services.ConsoleWriters;
using Spectre.Console;
using Spectre.Console.Cli;
using System.Globalization;

namespace DataCrafter.Commands.DataFrame.CsvStatistics;
internal sealed class CsvStatisticsCommand : Command<CsvStatisticsCommandSettings>
{
    private readonly IAnsiConsole _ansiConsole;
    private readonly IDataFrameColumnConsoleWriter _dataFrameColumnConsoleWriter;

    public CsvStatisticsCommand(IAnsiConsole ansiConsole, IDataFrameColumnConsoleWriter dataFrameColumnConsoleWriter)
    {
        _ansiConsole = ansiConsole;
        _dataFrameColumnConsoleWriter = dataFrameColumnConsoleWriter;
    }

    public override int Execute(CommandContext context, CsvStatisticsCommandSettings settings)
    {
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
        var columnsasRows = settings.ColumnsAsRows.IsSet ? settings.ColumnsAsRows.Value : statistics.Count > 7;
        _dataFrameColumnConsoleWriter.PrintColumnsToConsole(statistics, columnsasRows);

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
