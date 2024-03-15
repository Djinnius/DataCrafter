using DataCrafter.Services.ConsoleWriters;
using FluentValidation;
using Spectre.Console.Cli;

namespace DataCrafter.Commands.Distributions.DistributionTypesSummary;
internal sealed class PrintDistributionTypeSummaryTableCommandSettingsValidator : AbstractValidator<PrintDistributionTypeSummaryTableCommandSettings>
{
    public PrintDistributionTypeSummaryTableCommandSettingsValidator()
    {
        RuleFor(x => x.ColourBy)
            .Must(BeValidEnumValue).When(x => x.ColourBy.IsSet)
            .WithMessage("Invalid value for ColourBy.");

        RuleFor(x => x.OrderBy)
            .Must(BeValidEnumValue).When(x => x.OrderBy.IsSet)
            .WithMessage("Invalid value for OrderBy.");
    }

    private bool BeValidEnumValue(FlagValue<DistributionInfoType> value)
        => value.IsSet && Enum.IsDefined(typeof(DistributionInfoType), value.Value);
}
