using System.ComponentModel;
using Spectre.Console.Cli;

namespace DataCrafter.Commands.DataFrameColumns.Print;

internal class PrintDataFrameColumnsCommandSettings : CommandSettings
{
    [CommandOption("-c|--columnsAsRows [bool]")]
    [Description("Print columns as rows.")]
    public FlagValue<bool> ColumnsAsRows { get; set; } = null!;
}
