using System.Text.Json;
using DataCrafter.Entities;
using DataCrafter.Services.Repositories;
using Spectre.Console;
using Spectre.Console.Cli;

namespace DataCrafter.Commands.DataFrameColumns.Load;
internal sealed class LoadDataFrameColumnsCommand : Command<LoadDataFrameColumnsCommandSettings>
{
    private readonly ITableSchemaRepository _tableSchemaRepository;
    private readonly IAnsiConsole _ansiConsole;

    public LoadDataFrameColumnsCommand(
        ITableSchemaRepository tableSchemaRepository,
        IAnsiConsole ansiConsole)
    {
        _tableSchemaRepository = tableSchemaRepository;
        _ansiConsole = ansiConsole;
    }

    public override int Execute(CommandContext context, LoadDataFrameColumnsCommandSettings settings)
    {
        var inputFileName = GetInputFileName(settings.Input);
        var inputPath = GetInputPath(settings.Input);

        try
        {
            var fullInputPath = Path.Combine(inputPath, inputFileName);

            if (!File.Exists(fullInputPath))
            {
                _ansiConsole.MarkupLine($"[red]Error: File not found: {fullInputPath}[/]");
                return -1;
            }

            var json = File.ReadAllText(fullInputPath);
            var dataFrameColumns = JsonSerializer.Deserialize<List<IDataFrameColumn>>(json);

            if (dataFrameColumns == null || dataFrameColumns.Count == 0)
            {
                _ansiConsole.MarkupLine($"[red]No columns loaded from: {fullInputPath}[/]");
                return -1;
            }

            _tableSchemaRepository.DeleteAllDataFrameColumns();
            _tableSchemaRepository.OverwriteAllDataFrameColumns(dataFrameColumns);

            _ansiConsole.MarkupLine($"[green]Columns successfully loaded from: {fullInputPath}[/]");
            return 0;
        }
        catch (Exception ex)
        {
            _ansiConsole.MarkupLine($"[red]Error loading columns.[/]");
            _ansiConsole.WriteException(ex, ExceptionFormats.ShortenEverything);
            return -1;
        }
    }

    private string GetInputFileName(FlagValue<string> input)
    {
        if (input.IsSet)
        {
            var fileName = Path.GetFileNameWithoutExtension(input.Value) ?? input.Value;
            return $"{fileName}.json";
        }

        return "columns.json";
    }

    private string GetInputPath(FlagValue<string> input)
    {
        if (input.IsSet)
        {
            var directory = Path.GetDirectoryName(input.Value);
            return string.IsNullOrEmpty(directory) ? Directory.GetCurrentDirectory() : directory;
        }

        return Directory.GetCurrentDirectory();
    }
}
