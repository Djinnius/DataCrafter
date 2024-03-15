using Spectre.Console.Cli;
using System.ComponentModel;

namespace DataCrafter.Commands.DataFrame.Generate;
internal sealed class GenerateDataCommandSettings : CommandSettings
{
    [CommandArgument(0, "<Count>")]
    [Description("Number of rows to generate.")]
    public int Count { get; set; }

    [CommandOption("-o|--output [string]")]
    [Description("Output path and file name.")]
    public FlagValue<string> Output { get; set; } = null!;
}
