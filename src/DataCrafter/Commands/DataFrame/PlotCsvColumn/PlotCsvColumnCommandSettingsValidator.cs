using FluentValidation;

namespace DataCrafter.Commands.DataFrame.PlotCsvColumn;
internal sealed class PlotCsvColumnCommandSettingsValidator : AbstractValidator<PlotCsvColumnCommandSettings>
{
    public PlotCsvColumnCommandSettingsValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name cannot be empty or whitespace.");

        When(x => x.Buckets.IsSet, () =>
        {
            RuleFor(x => x.Buckets.Value)
                .GreaterThan(1).LessThan(200).WithMessage("Buckets must be greater than 1 and less than 200.");
        });
    }
}
