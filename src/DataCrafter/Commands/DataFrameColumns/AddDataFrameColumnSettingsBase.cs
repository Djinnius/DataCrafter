using System.ComponentModel;
using Spectre.Console.Cli;

namespace DataCrafter.Commands.DataFrameColumns;
internal class AddDataFrameColumnSettingsBase : CommandSettings
{
    [CommandArgument(0, "<NAME>")]
    [Description("New column unique name.")]
    public string Name { get; set; } = string.Empty;

    [CommandArgument(1, "<TYPE>")]
    [Description("Type of data to generate.")]
    public string Type { get; set; } = "double";

    [CommandOption("-o|--ordinal [int]")]
    [Description("Ordinal of the column.")]
    public FlagValue<int> Ordinal { get; set; } = null!;

    [CommandOption("-s|--seed [int]")]
    [Description("Seed when using deterministic data generation.")]
    public FlagValue<int> Seed { get; set; } = null!;
}
