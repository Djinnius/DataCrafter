using DataCrafter.Entities;
using DataCrafter.Pocos.DistributionConfigurations;
using DataCrafter.Services.ConsoleWriters;
using DataCrafter.Services.Repositories;
using FluentValidation;
using Spectre.Console;
using Spectre.Console.Cli;

namespace DataCrafter.Commands.DataFrameColumns.LogNormal;

internal sealed class AddLogNormalDataFrameColumnCommand : AddDataFrameColumnCommandBase<AddLogNormalDataFrameColumnCommandSettings>
{
    private readonly ITableSchemaRepository _tableSchemaRepository;
    private readonly IDataFrameColumnConsoleWriter _dataFrameColumnConsoleWriter;

    public AddLogNormalDataFrameColumnCommand(
        ITableSchemaRepository tableSchemaRepository,
        IDataFrameColumnConsoleWriter dataFrameColumnConsoleWriter,
        IValidator<AddLogNormalDataFrameColumnCommandSettings> validator,
        IAnsiConsole ansiConsole) : base(tableSchemaRepository, validator, ansiConsole)
    {
        _tableSchemaRepository = tableSchemaRepository;
        _dataFrameColumnConsoleWriter = dataFrameColumnConsoleWriter;
    }

    public override int Execute(CommandContext context, AddLogNormalDataFrameColumnCommandSettings settings)
    {
        if (ValidateSettings(settings) < 0 || CheckDataFrameColumnForOverwrite(settings) < 0)
            return -1;

        var dataFrameColumn = new DataFrameColumn
        {
            Name = settings.Name,
            DataType = settings.Type,
            DistributionConfig = new LogNormalDistributionConfig { Mean = settings.Mean, StandardDeviation = settings.StandardDeviation },
            Seed = settings.Seed.IsSet ? settings.Seed.Value : 0,
            Ordinal = DetermineOrdinal(settings),
        };

        _tableSchemaRepository.UpsertDataFrameColumn(dataFrameColumn);
        _dataFrameColumnConsoleWriter.PrintColumnsToConsole(_tableSchemaRepository.GetAllDataFrameColumns());
        return 0;
    }
}
