﻿using DataCrafter.DistributionConfigurations;
using DataCrafter.Entities;
using DataCrafter.Services.ConsoleWriters;
using DataCrafter.Services.Repositories;
using FluentValidation;
using Spectre.Console;
using Spectre.Console.Cli;

namespace DataCrafter.Commands.DataFrameColumns.Exponential;
internal sealed class AddExponentialDataFrameColumnCommand : AddDataFrameColumnCommandBase<AddExponentialDataFrameColumnCommandSettings>
{
    private readonly ITableSchemaRepository _tableSchemaRepository;
    private readonly IDataFrameColumnConsoleWriter _dataFrameColumnConsoleWriter;

    public AddExponentialDataFrameColumnCommand(
        ITableSchemaRepository tableSchemaRepository,
        IDataFrameColumnConsoleWriter dataFrameColumnConsoleWriter,
        IValidator<AddExponentialDataFrameColumnCommandSettings> validator,
        IAnsiConsole ansiConsole) : base(tableSchemaRepository, validator, ansiConsole)
    {
        _tableSchemaRepository = tableSchemaRepository;
        _dataFrameColumnConsoleWriter = dataFrameColumnConsoleWriter;
    }

    public override int Execute(CommandContext context, AddExponentialDataFrameColumnCommandSettings settings)
    {
        if (ValidateSettings(settings) < 0 || CheckDataFrameColumnForOverwrite(settings) < 0)
            return -1;

        var dataFrameColumn = new DataFrameColumn
        {
            Name = settings.Name,
            Type = settings.Type,
            DistributionConfig = new ExponentialDistributionConfig { Rate = settings.Rate },
            Seed = settings.Seed.IsSet ? settings.Seed.Value : 0,
            Ordinal = DetermineOrdinal(settings),
        };

        _tableSchemaRepository.UpsertDataFrameColumn(dataFrameColumn);
        _dataFrameColumnConsoleWriter.PrintColumnsToConsole(_tableSchemaRepository.GetAllDataFrameColumns());
        return 0;
    }
}