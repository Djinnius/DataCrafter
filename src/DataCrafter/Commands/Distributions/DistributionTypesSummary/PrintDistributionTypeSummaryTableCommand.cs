using DataCrafter.Services.ConsoleWriters;
using DataCrafter.Services.Distributions;
using FluentValidation;
using Spectre.Console;
using Spectre.Console.Cli;

namespace DataCrafter.Commands.Distributions.DistributionTypesSummary;
internal sealed class PrintDistributionTypeSummaryTableCommand : Command<PrintDistributionTypeSummaryTableCommandSettings>
{
    private readonly IAnsiConsole _ansiConsole;
    private readonly IDistributionProvider _distributionProvider;
    private readonly IDistributionDetailsConsoleWriter _distributionDetailsConsoleWriter;
    private readonly IValidator<PrintDistributionTypeSummaryTableCommandSettings> _validator;

    public PrintDistributionTypeSummaryTableCommand(IAnsiConsole ansiConsole,
                                             IDistributionProvider distributionProvider,
                                             IDistributionDetailsConsoleWriter distributionDetailsConsoleWriter,
                                             IValidator<PrintDistributionTypeSummaryTableCommandSettings> validator)
    {
        _ansiConsole = ansiConsole;
        _distributionProvider = distributionProvider;
        _distributionDetailsConsoleWriter = distributionDetailsConsoleWriter;
        _validator = validator;
    }

    public override int Execute(CommandContext context, PrintDistributionTypeSummaryTableCommandSettings settings)
    {
        if (ValidateSettings(settings) < 0)
            return -1;

        var orderBy = settings.OrderBy.IsSet ? settings.OrderBy.Value : DistributionInfoType.Name;
        var colourBy = settings.ColourBy.IsSet ? settings.ColourBy.Value : DistributionInfoType.Name;

        var distributions = _distributionProvider.GetUnivariateDistributions();
        _distributionDetailsConsoleWriter.WriteDistributionMetaDataToConsole(distributions, colourBy, orderBy);

        return 0;
    }

    private int ValidateSettings(PrintDistributionTypeSummaryTableCommandSettings settings)
    {
        var validationResult = _validator.Validate(settings);

        if (!validationResult.IsValid)
        {
            foreach (var error in validationResult.Errors)
                _ansiConsole.MarkupLine($"[red]Validation Error: {error.ErrorMessage}[/]");

            return -1;
        }

        return 0;
    }
}
