using System.ComponentModel;
using Spectre.Console.Cli;

namespace DataCrafter.Commands.DataFrame.PlotCsvColumn;
internal sealed class PlotCsvColumnCommandSettings : CommandSettings
{
    [CommandArgument(0, "<Name>")]
    [Description("Name of column to plot.")]
    public string Name { get; set; } = string.Empty;

    [CommandOption("-i|--input [string]")]
    [Description("Input path and file name.")]
    public FlagValue<string> Input { get; set; } = null!;

    [CommandOption("-b|--buckets [string]")]
    [Description("The number of buckets to plot over.")]
    public FlagValue<int> Buckets { get; set; } = null!;
}
