using Spectre.Console.Cli;
using System.ComponentModel;

namespace DataCrafter.Commands.DataFrameColumns.Uniform;
internal sealed class AddUniformDataFrameColumnCommandSettings : AddDataFrameColumnSettingsBase
{
    [CommandArgument(2, "<MIN>")]
    [Description("Min.")]
    public double Min { get; set; }

    [CommandArgument(3, "<MAX>")]
    [Description("Max.")]
    public double Max { get; set; }
}
