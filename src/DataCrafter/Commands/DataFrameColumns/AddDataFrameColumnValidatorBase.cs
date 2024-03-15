using DataCrafter.Services.DataTypeServices;
using FluentValidation;
using Spectre.Console.Cli;

namespace DataCrafter.Commands.DataFrameColumns;

internal abstract class AddDataFrameColumnValidatorBase<TSettings> : AbstractValidator<TSettings>
    where TSettings : AddDataFrameColumnSettingsBase
{
    private readonly IDataTypeProvider _dataTypeProvider;

    protected AddDataFrameColumnValidatorBase(IDataTypeProvider dataTypeProvider)
    {
        _dataTypeProvider = dataTypeProvider;

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name cannot be empty.");

        RuleFor(x => x.Type)
            .Must(BeValidType).WithMessage("Type must be double or integer.");

        RuleFor(x => x.Ordinal)
            .Must(BeValidOrdinal).WithMessage("Ordinal must be positive if provided.")
            .When(x => x.Ordinal.IsSet);
    }

    private bool BeValidType(string type)
        => _dataTypeProvider.DoubleAliases.Contains(type) || _dataTypeProvider.IntAliases.Contains(type);

    private bool BeValidOrdinal(FlagValue<int> ordinal)
        => !ordinal.IsSet || ordinal.IsSet && ordinal.Value >= 0;
}
