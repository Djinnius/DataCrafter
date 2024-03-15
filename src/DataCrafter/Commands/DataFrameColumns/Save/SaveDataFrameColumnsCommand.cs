using System.Text.Json;
using DataCrafter.Services.Repositories;
using Spectre.Console;
using Spectre.Console.Cli;

namespace DataCrafter.Commands.DataFrameColumns.Save;
internal class SaveDataFrameColumnsCommand : Command<SaveDataFrameColumnsCommandSettings>
{
    private readonly ITableSchemaRepository _tableSchemaRepository;
    private readonly IAnsiConsole _ansiConsole;

    public SaveDataFrameColumnsCommand(
        ITableSchemaRepository tableSchemaRepository,
        IAnsiConsole ansiConsole)
    {
        _tableSchemaRepository = tableSchemaRepository;
        _ansiConsole = ansiConsole;
    }

    public override int Execute(CommandContext context, SaveDataFrameColumnsCommandSettings settings)
    {
        var dataFrameColumns = _tableSchemaRepository.GetAllDataFrameColumns().ToList();

        if (!dataFrameColumns.Any())
        {
            _ansiConsole.MarkupLine("[red]Error: No columns in configuration[/]");
            return -1;
        }

        var outputFileName = GetOutputFileName(settings.Output);
        var outputPath = GetOutputPath(settings.Output);

        try
        {
            var fullOutputPath = Path.Combine(outputPath, outputFileName);
            var json = JsonSerializer.Serialize(dataFrameColumns, new JsonSerializerOptions { WriteIndented = true });

            File.WriteAllText(fullOutputPath, json);

            _ansiConsole.MarkupLine($"[green]Columns successfully saved to: {fullOutputPath}[/]");
            return 0;
        }
        catch (Exception ex)
        {
            _ansiConsole.MarkupLine($"[red]Error saving columns.[/]");
            _ansiConsole.WriteException(ex, ExceptionFormats.ShortenEverything);
            return -1;
        }
    }

    private string GetOutputFileName(FlagValue<string> output)
    {
        if (output.IsSet)
        {
            var fileName = Path.GetFileNameWithoutExtension(output.Value) ?? output.Value;
            return $"{fileName}.json";
        }

        return "columns.json";
    }

    private string GetOutputPath(FlagValue<string> output)
    {
        if (output.IsSet)
        {
            var directory = Path.GetDirectoryName(output.Value);
            return string.IsNullOrEmpty(directory) ? Directory.GetCurrentDirectory() : directory;
        }

        return Directory.GetCurrentDirectory();
    }
}
