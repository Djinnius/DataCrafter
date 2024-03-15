using Spectre.Console.Cli;
using System.ComponentModel;

namespace DataCrafter.Commands.DataFrameColumns.Load;
internal sealed class LoadDataFrameColumnsCommandSettings : CommandSettings
{
    [CommandOption("-i|--input [string]")]
    [Description("Input path and file name.")]
    public FlagValue<string> Input { get; set; } = null!;
}
