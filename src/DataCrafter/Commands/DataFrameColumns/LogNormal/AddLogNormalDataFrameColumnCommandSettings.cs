using Spectre.Console.Cli;
using System.ComponentModel;

namespace DataCrafter.Commands.DataFrameColumns.LogNormal;
internal sealed class AddLogNormalDataFrameColumnCommandSettings : AddDataFrameColumnSettingsBase
{
    [CommandArgument(2, "<MEAN>")]
    [Description("Mean.")]
    public double Mean { get; set; }

    [CommandArgument(3, "<STANDARD DEVIATION>")]
    [Description("Standard Deviation.")]
    public double StandardDeviation { get; set; }
}
