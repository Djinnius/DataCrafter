using DataCrafter.Entities;
using DataCrafter.Pocos.DistributionConfigurations;
using DataCrafter.Services.ConsoleWriters;
using DataCrafter.Services.Repositories;
using FluentValidation;
using Spectre.Console;
using Spectre.Console.Cli;

namespace DataCrafter.Commands.DataFrameColumns.Gamma;
internal sealed class AddGammaDataFrameColumnCommand : AddDataFrameColumnCommandBase<AddGammaDataFrameColumnCommandSettings>
{
    private readonly ITableSchemaRepository _tableSchemaRepository;
    private readonly IDataFrameColumnConsoleWriter _dataFrameColumnConsoleWriter;

    public AddGammaDataFrameColumnCommand(
        ITableSchemaRepository tableSchemaRepository,
        IDataFrameColumnConsoleWriter dataFrameColumnConsoleWriter,
        IValidator<AddGammaDataFrameColumnCommandSettings> validator,
        IAnsiConsole ansiConsole) : base(tableSchemaRepository, validator, ansiConsole)
    {
        _tableSchemaRepository = tableSchemaRepository;
        _dataFrameColumnConsoleWriter = dataFrameColumnConsoleWriter;
    }

    public override int Execute(CommandContext context, AddGammaDataFrameColumnCommandSettings settings)
    {
        if (ValidateSettings(settings) < 0 || CheckDataFrameColumnForOverwrite(settings) < 0)
            return -1;

        var dataFrameColumn = new DataFrameColumn
        {
            Name = settings.Name,
            DataType = settings.Type,
            DistributionConfig = new GammaDistributionConfig { Shape = settings.Shape, Scale = settings.Scale },
            Seed = settings.Seed.IsSet ? settings.Seed.Value : 0,
            Ordinal = DetermineOrdinal(settings),
        };

        _tableSchemaRepository.UpsertDataFrameColumn(dataFrameColumn);
        _dataFrameColumnConsoleWriter.PrintColumnsToConsole(_tableSchemaRepository.GetAllDataFrameColumns());
        return 0;
    }
}
