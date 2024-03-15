using Spectre.Console.Cli;
using System.ComponentModel;

namespace DataCrafter.Commands.DataFrameColumns.Exponential;
internal sealed class AddExponentialDataFrameColumnCommandSettings : AddDataFrameColumnSettingsBase
{
    [CommandArgument(2, "<RATE>")]
    [Description("Rate.")]
    public double Rate { get; set; }
}
