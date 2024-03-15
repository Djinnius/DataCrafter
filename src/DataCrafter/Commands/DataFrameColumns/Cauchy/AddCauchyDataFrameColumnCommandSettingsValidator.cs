using DataCrafter.Services.DataTypeServices;
using FluentValidation;

namespace DataCrafter.Commands.DataFrameColumns.Cauchy;

internal sealed class AddCauchyDataFrameColumnCommandSettingsValidator : AddDataFrameColumnValidatorBase<AddCauchyDataFrameColumnCommandSettings>
{
    public AddCauchyDataFrameColumnCommandSettingsValidator(IDataTypeProvider dataTypeProvider)
        : base(dataTypeProvider)
    {
        RuleFor(x => x.Location)
            .NotEmpty().WithMessage("Location cannot be empty.")
            .Must(BeValidDouble).WithMessage("Location must be a valid double value and not Infinity or NaN.");

        RuleFor(x => x.Scale)
            .NotEmpty().WithMessage("Scale cannot be empty.")
            .Must(BeValidDouble).WithMessage("Scale must be a valid double value and not Infinity or NaN.");
    }

    private bool BeValidDouble(double value)
        => !double.IsNaN(value) && !double.IsInfinity(value);
}
