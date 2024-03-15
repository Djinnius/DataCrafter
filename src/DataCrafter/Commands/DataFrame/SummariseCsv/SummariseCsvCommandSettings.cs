using System.ComponentModel;
using Spectre.Console.Cli;

namespace DataCrafter.Commands.DataFrame.SummariseCsv;
internal sealed class SummariseCsvCommandSettings : CommandSettings
{
    [CommandOption("-i|--input [string]")]
    [Description("Input path and file name.")]
    public FlagValue<string> Input { get; set; } = null!;
}
