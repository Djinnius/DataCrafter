using DataCrafter.Services.DataTypeServices;
using FluentValidation;

namespace DataCrafter.Commands.DataFrameColumns.Uniform;
internal sealed class AddUniformDataFrameColumnCommandSettingsValidator : AddDataFrameColumnValidatorBase<AddUniformDataFrameColumnCommandSettings>
{
    public AddUniformDataFrameColumnCommandSettingsValidator(IDataTypeProvider dataTypeProvider)
        : base(dataTypeProvider)
    {
        RuleFor(x => x.Min)
            .Must(BeValidDouble).WithMessage("Min must be a valid double value and not Infinity or NaN.");

        RuleFor(x => x.Max)
            .Must(BeValidDouble).WithMessage("Max must be a valid double value and not Infinity or NaN.");

        When(x => BeValidDouble(x.Min) && BeValidDouble(x.Max), () =>
        {
            RuleFor(x => x.Min).LessThan(x => x.Max).WithMessage("Min value must be less than Max value.");
            RuleFor(x => x.Max).GreaterThan(x => x.Min).WithMessage("Max value must be greater than Min value.");
        });
    }

    private bool BeValidDouble(double value)
        => !double.IsNaN(value) && !double.IsInfinity(value);
}
