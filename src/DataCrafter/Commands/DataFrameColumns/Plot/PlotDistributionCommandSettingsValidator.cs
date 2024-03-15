using FluentValidation;

namespace DataCrafter.Commands.DataFrameColumns.Plot;
internal sealed class PlotDistributionCommandSettingsValidator : AbstractValidator<PlotDistributionCommandSettings>
{
    public PlotDistributionCommandSettingsValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name cannot be empty or whitespace.");

        When(x => x.Percentile.IsSet, () =>
        {
            RuleFor(x => x.Percentile.Value)
                .GreaterThan(0).LessThan(1).WithMessage("Percentile must be greater than 0 and less than 1.");
        });

        When(x => x.Buckets.IsSet, () =>
        {
            RuleFor(x => x.Buckets.Value)
                .GreaterThan(1).LessThan(200).WithMessage("Buckets must be greater than 1 and less than 200.");
        });
    }
}
