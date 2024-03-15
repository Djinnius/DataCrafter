using DataCrafter.Services.Repositories;
using Spectre.Console;
using Spectre.Console.Cli;

namespace DataCrafter.Commands.DataFrameColumns.Clear;

internal class ClearDataFrameColumnsCommand : Command
{
    private readonly ITableSchemaRepository _tableSchemaRepository;
    private readonly IAnsiConsole _ansiConsole;

    public ClearDataFrameColumnsCommand(ITableSchemaRepository tableSchemaRepository, IAnsiConsole ansiConsole)
    {
        _tableSchemaRepository = tableSchemaRepository;
        _ansiConsole = ansiConsole;
    }

    public override int Execute(CommandContext context)
    {
        if (!_tableSchemaRepository.GetAllDataFrameColumns().Any())
        {
            _ansiConsole.MarkupLine("[red]Error: No columns in configuration[/].");
            return -1;
        }

        _ansiConsole.MarkupLine($"The following columns {string.Join(",", _tableSchemaRepository.GetAllDataFrameColumns().Select(x => $"[bold yellow]{x.Name}[/]"))} have been cleared.");

        _tableSchemaRepository.DeleteAllDataFrameColumns();

        return 0;
    }
}
