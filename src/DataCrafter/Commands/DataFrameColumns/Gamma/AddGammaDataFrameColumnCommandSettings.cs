using Spectre.Console.Cli;
using System.ComponentModel;

namespace DataCrafter.Commands.DataFrameColumns.Gamma;
internal sealed class AddGammaDataFrameColumnCommandSettings : AddDataFrameColumnSettingsBase
{
    [CommandArgument(2, "<SHAPE>")]
    [Description("Shape. Represents if the distribution is skewed or symetric.")]
    public double Shape { get; set; }

    [CommandArgument(3, "<SCALE>")]
    [Description("Scale. Represents the spread or concentration of the distribution.")]
    public double Scale { get; set; }
}
