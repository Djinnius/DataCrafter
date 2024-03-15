using DataCrafter.Services.DataTypeServices;
using FluentValidation;

namespace DataCrafter.Commands.DataFrameColumns.Normal;

internal sealed class AddNormalDataFrameColumnCommandSettingsValidator : AddDataFrameColumnValidatorBase<AddNormalDataFrameColumnCommandSettings>
{
    public AddNormalDataFrameColumnCommandSettingsValidator(IDataTypeProvider dataTypeProvider)
        : base(dataTypeProvider)
    {
        RuleFor(x => x.Mean)
            .NotEmpty().WithMessage("Mean cannot be empty.")
            .Must(BeValidDouble).WithMessage("Mean must be a valid double value and not Infinity or NaN.");

        RuleFor(x => x.StandardDeviation)
            .NotEmpty().WithMessage("Standard deviation cannot be empty.")
            .Must(BeValidDouble).WithMessage("Standard deviation must be a valid double value and not Infinity or NaN.")
            .GreaterThan(0).WithMessage("Standard deviation must be a positive number.");
    }

    private bool BeValidDouble(double value)
        => !double.IsNaN(value) && !double.IsInfinity(value);
}
