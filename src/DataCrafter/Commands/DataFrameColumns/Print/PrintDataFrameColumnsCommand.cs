﻿using DataCrafter.Services.ConsoleWriters;
using DataCrafter.Services.Repositories;
using MathNet.Numerics.Statistics;
using Spectre.Console;
using Spectre.Console.Cli;

namespace DataCrafter.Commands.DataFrameColumns.Print;
internal sealed class PrintDataFrameColumnsCommand : Command<PrintDataFrameColumnsCommandSettings>
{
    private readonly ITableSchemaRepository _tableSchemaRepository;
    private readonly IDataFrameColumnConsoleWriter _dataFrameColumnConsoleWriter;
    private readonly IAnsiConsole _ansiConsole;

    public PrintDataFrameColumnsCommand(
        ITableSchemaRepository tableSchemaRepository,
        IDataFrameColumnConsoleWriter dataFrameColumnConsoleWriter,
        IAnsiConsole ansiConsole)
    {
        _tableSchemaRepository = tableSchemaRepository;
        _dataFrameColumnConsoleWriter = dataFrameColumnConsoleWriter;
        _ansiConsole = ansiConsole;
    }

    public override int Execute(CommandContext context, PrintDataFrameColumnsCommandSettings settings)
    {
        var columns = _tableSchemaRepository.GetAllDataFrameColumns().ToList();

        if (columns.Count == 0)
        {
            _ansiConsole.MarkupLine("[red]Error: No columns in configuration[/]");
            return -1;
        }

        var columnsasRows = settings.ColumnsAsRows.IsSet ? settings.ColumnsAsRows.Value : columns.Count > 7;
        _dataFrameColumnConsoleWriter.PrintColumnsToConsole(columns, columnsasRows);

        return 0;
    }
}