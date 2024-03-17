using Spectre.Console;
using Spectre.Console.Cli;

namespace DataCrafter.Commands.StatisticalDefinitions;

internal sealed class SkewnessKurtosisDetailsCommand : AsyncCommand
{
    private readonly IAnsiConsole _ansiConsole;

    public SkewnessKurtosisDetailsCommand(IAnsiConsole ansiConsole)
    {
        _ansiConsole = ansiConsole ?? throw new ArgumentNullException(nameof(ansiConsole));
    }

    public override async Task<int> ExecuteAsync(CommandContext context)
    {
        PrintSkewnessExplanation();
        PrintKurtosisExplanation();
        return 0;
    }

    private void PrintSkewnessExplanation()
    {
        _ansiConsole.WriteLine();
        _ansiConsole.MarkupLine("[underline]Skewness Explanation[/]");
        _ansiConsole.WriteLine();
        _ansiConsole.WriteLine("Skewness measures the asymmetry of the distribution of values in a dataset.");
        _ansiConsole.WriteLine("A skewness value of 0 indicates a symmetric distribution.");
        _ansiConsole.WriteLine("Positive skewness indicates a right-skewed distribution (tail on the right side of the distribution).");
        _ansiConsole.WriteLine("Negative skewness indicates a left-skewed distribution (tail on the left side of the distribution).");
    }

    private void PrintKurtosisExplanation()
    {
        _ansiConsole.WriteLine();
        _ansiConsole.MarkupLine("[underline]Kurtosis Explanation[/]");
        _ansiConsole.WriteLine();
        _ansiConsole.WriteLine("Kurtosis measures the peakedness or flatness of the distribution of values in a dataset.");
        _ansiConsole.WriteLine("A kurtosis value of 3 (or sometimes 0) indicates a normal distribution (mesokurtic).");
        _ansiConsole.WriteLine("Kurtosis greater than 3 indicates heavier tails (leptokurtic distribution).");
        _ansiConsole.WriteLine("Kurtosis less than 3 indicates lighter tails (platykurtic distribution).");
    }
}
