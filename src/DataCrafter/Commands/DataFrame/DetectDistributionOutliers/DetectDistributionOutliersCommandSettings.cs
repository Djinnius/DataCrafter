using Spectre.Console.Cli;
using System.ComponentModel;

namespace DataCrafter.Commands.DataFrame.DetectDistributionOutliers;
internal class DetectDistributionOutliersCommandSettings : CommandSettings
{
    [CommandArgument(0, "<Name>")]
    [Description("Name of column to plot.")]
    public string Name { get; set; } = string.Empty;

    [CommandArgument(1, "<Distribution>")]
    [Description("Name of distribution to fit.")]
    public string Distribution { get; set; } = string.Empty;

    [CommandOption("-i|--input [string]")]
    [Description("Input path and file name.")]
    public FlagValue<string> Input { get; set; } = null!;

    [CommandOption("-t|--threshold [int]")]
    [Description("P threshold under which values will flag as outliers.")]
    public FlagValue<double> Threshold { get; set; } = null!;
}
