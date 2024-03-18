using FluentValidation;

namespace DataCrafter.Commands.DataFrame.ShapiroWilk;
internal sealed class ShapiroWilkTestCommandSettingsValidator : AbstractValidator<ShapiroWilkTestCommandSettings>
{
    public ShapiroWilkTestCommandSettingsValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name cannot be empty or whitespace.");
    }
}
