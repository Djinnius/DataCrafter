using DataCrafter.Commands.DataFrameColumns.Print;
using DataCrafter.Entities;
using DataCrafter.Pocos.DistributionConfigurations;
using DataCrafter.Services.ConsoleWriters;
using DataCrafter.Services.Repositories;
using FluentAssertions.Execution;
using Spectre.Console;
using Spectre.Console.Cli;
using Spectre.Console.Testing;

namespace DataCrafter.Tests.Commands.DataFrameColumns.Print;
public sealed class PrintDataFrameColumnsCommandTests
{
    [Fact]
    public void PrintDataFrameColumnsCommand_ShouldOutputErrorMessage_WhenNoColumnsConfigured()
    {
        // arrange
        var console = new TestConsole().Width(120);
        var tableSchemaRepository = Substitute.For<ITableSchemaRepository>();
        var dataFrameColumnConsoleWriter = Substitute.For<IDataFrameColumnConsoleWriter>();
        var _remainingArgs = Substitute.For<IRemainingArguments>();

        // -- configure returns
        tableSchemaRepository.GetAllDataFrameColumns().Returns(new List<DataFrameColumn>());

        // -- configure command
        var command = new PrintDataFrameColumnsCommand(tableSchemaRepository, dataFrameColumnConsoleWriter, console);
        var commandSettings = new PrintDataFrameColumnsCommandSettings { ColumnsAsRows = new FlagValue<bool> { Value = true } };
        var context = new CommandContext(_remainingArgs, "print", null);

        // act
        var result = command.Execute(context, commandSettings);
        var text = console.Output;

        // assert
        using var scope = new AssertionScope();
        result.Should().Be(-1, "the user should be warned the columns list is empty and cannot be printed.");
        text.Should().Contain("Error: No columns in configuration", "this is a useful warning to end users defined in the command.");
    }

    [Fact]
    public void PrintDataFrameColumnsCommand_ShouldCallDataFrameColumnConsoleWriter_WhenColumnsConfigured()
    {
        // arrange
        var console = new TestConsole().Width(120);
        var tableSchemaRepository = Substitute.For<ITableSchemaRepository>();
        var dataFrameColumnConsoleWriter = Substitute.For<IDataFrameColumnConsoleWriter>();
        var _remainingArgs = Substitute.For<IRemainingArguments>();

        // -- configure returns
        tableSchemaRepository.GetAllDataFrameColumns().Returns(new List<DataFrameColumn> {  new DataFrameColumn
            {
                Name = "Column1",
                DataType = "Double",
                DistributionConfig = new NormalDistributionConfig { Mean = 5, StandardDeviation = 1 },
                Seed = 42,
                Ordinal = 1
            }});

        // -- configure command
        var command = new PrintDataFrameColumnsCommand(tableSchemaRepository, dataFrameColumnConsoleWriter, console);
        var commandSettings = new PrintDataFrameColumnsCommandSettings { ColumnsAsRows = new FlagValue<bool> { Value = true } };
        var context = new CommandContext(_remainingArgs, "print", null);

        // act
        var result = command.Execute(context, commandSettings);
        var text = console.Output;

        // assert
        using var scope = new AssertionScope();
        result.Should().Be(0, "the command is successfully calling IDataFrameColumnConsoleWriter.");
        text.Should().BeEmpty("no output is expected from the command directly.");
    }
}
