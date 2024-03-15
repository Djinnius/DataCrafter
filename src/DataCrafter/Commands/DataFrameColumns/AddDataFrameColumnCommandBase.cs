using DataCrafter.Services.Repositories;
using FluentValidation;
using Spectre.Console;
using Spectre.Console.Cli;

namespace DataCrafter.Commands.DataFrameColumns;
internal class AddDataFrameColumnCommandBase<TSettings> : Command<TSettings> where TSettings : AddDataFrameColumnSettingsBase
{
    private readonly ITableSchemaRepository _tableSchemaRepository;
    private readonly IValidator<TSettings> _validator;
    private readonly IAnsiConsole _ansiConsole;

    public AddDataFrameColumnCommandBase(
        ITableSchemaRepository tableSchemaRepository,
        IValidator<TSettings> validator,
        IAnsiConsole ansiConsole)
    {
        _tableSchemaRepository = tableSchemaRepository ?? throw new ArgumentNullException(nameof(tableSchemaRepository));
        _validator = validator ?? throw new ArgumentNullException(nameof(validator));
        _ansiConsole = ansiConsole;
    }

    public override int Execute(CommandContext context, TSettings settings)
        => throw new NotImplementedException();

    protected int ValidateSettings(TSettings settings)
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

    protected int CheckDataFrameColumnForOverwrite(TSettings settings)
    {
        var existingDataFrameColumn = _tableSchemaRepository.GetDataFrameColumnByName(settings.Name);

        if (existingDataFrameColumn is null)
            return 0;

        var confirmation = _ansiConsole.Confirm("Column exists. Do you want to proceed in overwriting?");

        if (confirmation)
        {
            _ansiConsole.MarkupLine("[green]Confirmed![/]");
            return 0;
        }

        _ansiConsole.MarkupLine("[red]Cancelled![/]");
        return -1;
    }

    protected int DetermineOrdinal(TSettings settings)
    {
        var dataFrameColumns = _tableSchemaRepository.GetAllDataFrameColumns();
        var maxOrdinal = dataFrameColumns.Any() ? dataFrameColumns.Max(x => x.Ordinal) : -1;

        if (!settings.Ordinal.IsSet || settings.Ordinal.Value > maxOrdinal)
            return maxOrdinal + 1;

        // Adding a data frame column to the middle of the series.
        // Increment all data frame column ordinals with value equal to or greater than the provided ordinal
        foreach (var dataFrameColumn in dataFrameColumns.Where(x => x.Ordinal >= settings.Ordinal.Value))
            dataFrameColumn.Ordinal++;

        return settings.Ordinal.Value;
    }
}
