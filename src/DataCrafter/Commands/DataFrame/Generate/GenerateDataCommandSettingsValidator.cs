using FluentValidation;

namespace DataCrafter.Commands.DataFrame.Generate;
internal class GenerateDataCommandSettingsValidator : AbstractValidator<GenerateDataCommandSettings>
{
    public GenerateDataCommandSettingsValidator()
    {
        RuleFor(x => x.Count).GreaterThan(1).WithMessage("Number of rows generated must be greater than 0");
    }
}
