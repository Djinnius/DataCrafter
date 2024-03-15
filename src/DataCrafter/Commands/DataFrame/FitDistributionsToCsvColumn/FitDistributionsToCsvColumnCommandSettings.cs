using System.ComponentModel;
using Spectre.Console.Cli;

namespace DataCrafter.Commands.DataFrame.FitDistributionsToCsvColumn;
internal sealed class FitDistributionsToCsvColumnCommandSettings : CommandSettings
{
    [CommandArgument(0, "<Name>")]
    [Description("Name of column to plot.")]
    public string Name { get; set; } = string.Empty;

    [CommandOption("-i|--input [string]")]
    [Description("Input path and file name.")]
    public FlagValue<string> Input { get; set; } = null!;
}
