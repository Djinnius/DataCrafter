using System.ComponentModel;
using Spectre.Console.Cli;

namespace DataCrafter.Commands.DataFrameColumns.Plot;
internal sealed class PlotDistributionCommandSettings : CommandSettings
{
    [CommandArgument(0, "<Name>")]
    [Description("Name of column to plot.")]
    public string Name { get; set; } = string.Empty;

    [CommandOption("-p|--percentile [string]")]
    [Description("The percentile to plot over.")]
    public FlagValue<double> Percentile { get; set; } = null!;

    [CommandOption("-b|--buckets [string]")]
    [Description("The number of buckets to plot over.")]
    public FlagValue<int> Buckets { get; set; } = null!;
}
