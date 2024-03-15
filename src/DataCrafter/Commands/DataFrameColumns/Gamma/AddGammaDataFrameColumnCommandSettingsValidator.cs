using DataCrafter.Services.DataTypeServices;
using FluentValidation;

namespace DataCrafter.Commands.DataFrameColumns.Gamma;
internal sealed class AddGammaDataFrameColumnCommandSettingsValidator : AddDataFrameColumnValidatorBase<AddGammaDataFrameColumnCommandSettings>
{
    public AddGammaDataFrameColumnCommandSettingsValidator(IDataTypeProvider dataTypeProvider)
        : base(dataTypeProvider)
    {
        RuleFor(x => x.Shape)
            .NotEmpty().WithMessage("Shape cannot be zero.")
            .Must(BeValidDouble).WithMessage("Shape must be a valid double value and not Infinity or NaN.")
            .GreaterThan(0).WithMessage("Shape must be a positive number.");

        RuleFor(x => x.Scale)
            .NotEmpty().WithMessage("Scale cannot be zero.")
            .Must(BeValidDouble).WithMessage("Scale must be a valid double value and not Infinity or NaN.")
            .GreaterThan(0).WithMessage("Scale must be a positive number.");
    }

    private bool BeValidDouble(double value)
        => !double.IsNaN(value) && !double.IsInfinity(value);
}
