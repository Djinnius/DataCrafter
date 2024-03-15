using System.Globalization;
using CsvHelper;
using Spectre.Console;
using Spectre.Console.Cli;

namespace DataCrafter.Commands.DataFrame.SummariseCsv;
internal class SummariseCsvCommand : Command<SummariseCsvCommandSettings>
{
    private readonly IAnsiConsole _ansiConsole;

    public SummariseCsvCommand(IAnsiConsole ansiConsole)
    {
        _ansiConsole = ansiConsole;
    }

    public override int Execute(CommandContext context, SummariseCsvCommandSettings settings)
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

        var headers = AnalyseCsv(inputFilePath, out var columnDataTypes, out var rowCount, out var fileSize);

        if (!headers.Any())
        {
            _ansiConsole.MarkupLine($"[yellow]Warning:[/] No headers found in file.");
            return -1;
        }

        // Output CSV headers and their inferred data types
        _ansiConsole.MarkupLine("[bold]CSV Headers and Data Types:[/]");
        foreach (var header in headers)
        {
            var dataType = columnDataTypes.ContainsKey(header) ? columnDataTypes[header].ToString() : "Unknown";
            _ansiConsole.MarkupLine($"[yellow]{header}[/]: {dataType}");
        }

        // Output total row count
        _ansiConsole.MarkupLine($"[bold]Total Rows:[/] {rowCount}");
        _ansiConsole.MarkupLine($"[bold]File Size:[/] {fileSize}");

        return 0;
    }

    private IEnumerable<string> AnalyseCsv(string inputFilePath, out Dictionary<string, DataType> columnDataTypes, out int rowCount, out string fileSize)
    {
        rowCount = 0;
        columnDataTypes = new Dictionary<string, DataType>();

        using var reader = new StreamReader(inputFilePath);
        using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);
        csv.Read();
        csv.ReadHeader();
        var headers = csv.HeaderRecord;

        if (headers is null)
        {
            fileSize = "0 B";
            return Enumerable.Empty<string>();
        }

        // Analyze the first 10 values in each column for data type
        while (csv.Read())
        {
            if (rowCount < 10)
                AggregateDataTypes(csv, headers, columnDataTypes);

            rowCount++;
        }

        var fileInfo = new FileInfo(inputFilePath);
        long sizeInBytes = fileInfo.Length;
        fileSize = FormatFileSize(sizeInBytes);

        return headers;
    }

    private string FormatFileSize(long fileSizeInBytes)
    {
        const long kiloByte = 1024;
        const long megaByte = kiloByte * kiloByte;

        if (fileSizeInBytes < kiloByte)
            return $"{fileSizeInBytes} B";
        else if (fileSizeInBytes < megaByte)
            return $"{(double)fileSizeInBytes / kiloByte:0.##} KB";
        else
            return $"{(double)fileSizeInBytes / megaByte:0.##} MB";
    }

    private static void AggregateDataTypes(CsvReader csv, string[] headers, Dictionary<string, DataType> columnDataTypes)
    {
        foreach (var header in headers)
        {
            if (csv.TryGetField<int>(header, out _))
            {
                if (!columnDataTypes.ContainsKey(header))
                {
                    columnDataTypes[header] = DataType.Integer;
                }
            }
            else if (csv.TryGetField<double>(header, out _))
            {
                if (!columnDataTypes.ContainsKey(header))
                {
                    columnDataTypes[header] = DataType.Double;
                }
                else if (columnDataTypes[header] == DataType.Integer)
                    columnDataTypes[header] = DataType.Double;
            }
        }
    }

    private enum DataType
    {
        Integer,
        Double,
    }
}
