using FluentValidation;

namespace DataCrafter.Commands.DataFrame.FitDistributionsToCsvColumn;
internal sealed class FitDistributionsToCsvColumnCommandSettingsValidator : AbstractValidator<FitDistributionsToCsvColumnCommandSettings>
{
    public FitDistributionsToCsvColumnCommandSettingsValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name cannot be empty or whitespace.");
    }
}
