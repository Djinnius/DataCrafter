using DataCrafter.Services.DataTypeServices;
using FluentValidation;

namespace DataCrafter.Commands.DataFrameColumns.Exponential;
internal sealed class AddExponentialDataFrameColumnCommandSettingsValidator : AddDataFrameColumnValidatorBase<AddExponentialDataFrameColumnCommandSettings>
{
    public AddExponentialDataFrameColumnCommandSettingsValidator(IDataTypeProvider dataTypeProvider)
        : base(dataTypeProvider)
    {
        RuleFor(x => x.Rate)
            .NotEmpty().WithMessage("Rate cannot be zero.")
            .Must(BeValidDouble).WithMessage("Rate must be a valid double value and not Infinity or NaN.")
            .GreaterThan(0).WithMessage("Rate must be a positive number.");
    }

    private bool BeValidDouble(double value)
        => !double.IsNaN(value) && !double.IsInfinity(value);
}
