using System.ComponentModel;
using Spectre.Console.Cli;

namespace DataCrafter.Commands.DataFrame.CsvStatistics;
internal sealed class CsvStatisticsCommandSettings : CommandSettings
{
    [CommandOption("-i|--input [string]")]
    [Description("Input path and file name.")]
    public FlagValue<string> Input { get; set; } = null!;


    [CommandOption("-c|--columnsAsRows [bool]")]
    [Description("Print columns as rows.")]
    public FlagValue<bool> ColumnsAsRows { get; set; } = null!;
}
