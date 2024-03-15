using System.ComponentModel;
using DataCrafter.Services.ConsoleWriters;
using Spectre.Console.Cli;

namespace DataCrafter.Commands.Distributions.DistributionTypesSummary;
internal sealed class PrintDistributionTypeSummaryTableCommandSettings : CommandSettings
{
    [CommandOption("-c|--colourby [string]")]
    [Description("The distribution property to colour.")]
    public FlagValue<DistributionInfoType> ColourBy { get; set; } = null!;

    [CommandOption("-o|--orderby [string]")]
    [Description("The distribution property to order by.")]
    public FlagValue<DistributionInfoType> OrderBy { get; set; } = null!;
}
