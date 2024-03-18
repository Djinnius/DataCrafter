using Spectre.Console.Cli;
using System.ComponentModel;

namespace DataCrafter.Commands.DataFrame.ShapiroWilk;
internal sealed class ShapiroWilkTestCommandSettings : CommandSettings
{
    [CommandArgument(0, "<Name>")]
    [Description("Name of column to plot.")]
    public string Name { get; set; } = string.Empty;

    [CommandOption("-i|--input [string]")]
    [Description("Input path and file name.")]
    public FlagValue<string> Input { get; set; } = null!;
}
