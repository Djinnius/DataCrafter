using Accord.Statistics.Distributions;
using DataCrafter.Services.ConsoleWriters;
using DataCrafter.Services.Repositories;
using FluentValidation;
using Spectre.Console;
using Spectre.Console.Cli;

namespace DataCrafter.Commands.DataFrameColumns.Plot;
internal sealed class PlotDistributionCommand : Command<PlotDistributionCommandSettings>
{
    private readonly IAnsiConsole _ansiConsole;
    private readonly IValidator<PlotDistributionCommandSettings> _validator;
    private readonly ITableSchemaRepository _tableSchemaRepository;
    private readonly IDistributionPlotterConsoleWriter _distributionPlotterConsoleWriter;

    public PlotDistributionCommand(
        IAnsiConsole ansiConsole,
        IValidator<PlotDistributionCommandSettings> validator,
        ITableSchemaRepository tableSchemaRepository,
        IDistributionPlotterConsoleWriter distributionPlotterConsoleWriter)
    {
        _ansiConsole = ansiConsole;
        _validator = validator;
        _tableSchemaRepository = tableSchemaRepository;
        _distributionPlotterConsoleWriter = distributionPlotterConsoleWriter;
    }

    public override int Execute(CommandContext context, PlotDistributionCommandSettings settings)
    {
        if (ValidateSettings(settings) < 0)
            return -1;

        var columns = _tableSchemaRepository.GetAllDataFrameColumns();

        if (!columns.Any())
        {
            _ansiConsole.MarkupLine($"[red]No columns in schema.[/]");
            _ansiConsole.MarkupLine($"Add columns with [bold yellow]datacrafter columns add[/].");
            return -1;
        }

        var column = columns.FirstOrDefault(x => x.Name == settings.Name);

        if (column is null)
        {
            _ansiConsole.MarkupLine($"[red]No column with name {settings.Name} found in schema.[/]");
            _ansiConsole.MarkupLine($"The following columns {string.Join(",", columns.Select(x => $"[bold yellow]{x.Name}[/]"))} are available.");
            return -1;
        }

        if (column.Distribution is not IUnivariateDistribution univariateDistribution)
        {
            _ansiConsole.MarkupLine($"[orange]{column.Distribution.ToString} is not a univariate distribution and is not supported with this command.[/]");
            return -1;
        }

        var percentile = settings.Percentile.IsSet ? settings.Percentile.Value : 0.9545; // default to 2 standard deviations of a normal distribution
        var bucketCount = settings.Buckets.IsSet ? settings.Buckets.Value : 20;

        _distributionPlotterConsoleWriter.PlotPdf(univariateDistribution, percentile, bucketCount, column.Name);
        return 1;
    }

    private int ValidateSettings(PlotDistributionCommandSettings settings)
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
