using System.ComponentModel;
using Spectre.Console.Cli;

namespace DataCrafter.Commands.DataFrameColumns.Cauchy;

internal sealed class AddCauchyDataFrameColumnCommandSettings : AddDataFrameColumnSettingsBase
{
    [CommandArgument(2, "<LOCATION>")]
    [Description("Location.")]
    public double Location { get; set; }

    [CommandArgument(3, "<SCALE>")]
    [Description("Scale.")]
    public double Scale { get; set; }
}

