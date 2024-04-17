using FluentValidation;

namespace DataCrafter.Commands.DataFrame.DetectDistributionOutliers;
internal sealed class DetectDistributionOutliersCommandSettingsValidator : AbstractValidator<DetectDistributionOutliersCommandSettings>
{
    public DetectDistributionOutliersCommandSettingsValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name cannot be empty or whitespace.");

        RuleFor(x => x.Distribution)
            .NotEmpty().WithMessage("Distribution cannot be empty or whitespace.");

        When(x => x.Threshold.IsSet, () =>
        {
            RuleFor(x => x.Threshold.Value)
                .GreaterThan(0).LessThan(1).WithMessage("Threshold must be greater than 0 and less than 1.");
        });
    }
}
