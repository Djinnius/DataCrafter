using System.ComponentModel;
using Spectre.Console.Cli;

namespace DataCrafter.Commands.DataFrameColumns.Normal;

internal sealed class AddNormalDataFrameColumnCommandSettings : AddDataFrameColumnSettingsBase
{
    [CommandArgument(2, "<MEAN>")]
    [Description("Mean.")]
    public double Mean { get; set; }

    [CommandArgument(3, "<STANDARD DEVIATION>")]
    [Description("Standard Deviation.")]
    public double StandardDeviation { get; set; }
}
