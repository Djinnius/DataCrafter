using System.ComponentModel;
using Spectre.Console.Cli;

namespace DataCrafter.Commands.DataFrameColumns.Save;
internal class SaveDataFrameColumnsCommandSettings : CommandSettings
{
    [CommandOption("-o|--output [string]")]
    [Description("Output path and file name.")]
    public FlagValue<string> Output { get; set; } = null!;
}
