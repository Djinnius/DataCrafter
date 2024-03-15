using Accord.Statistics.Distributions;
using Accord.Statistics.Distributions.Univariate;
using DataCrafter.Commands.DataFrame.CsvStatistics;
using DataCrafter.Entities;
using Spectre.Console;

namespace DataCrafter.Services.ConsoleWriters;

internal sealed class DataFrameColumnConsoleWriter : IDataFrameColumnConsoleWriter
{
    private readonly IAnsiConsole _ansiConsole;

    public DataFrameColumnConsoleWriter(IAnsiConsole ansiConsole)
    {
        _ansiConsole = ansiConsole;
    }

    public void PrintColumnsToConsole(IEnumerable<IDataFrameColumn> dataFrameColumns, bool columnsAsRows = false)
    {
        if (columnsAsRows)
        {
            PrintDistributionStatisticsAsRows(dataFrameColumns);
            return;
        }

        PrintDistributionStatisticsAsColumns(dataFrameColumns);
    }

    public void PrintColumnsToConsole(Dictionary<string, ColumnStatistics> columns, bool columnsAsRows = false)
    {
        if (columnsAsRows)
        {
            PrintColumnStatisticsAsRows(columns);
            return;
        }

        PrintColumnStatisticsAsColumns(columns);
    }

    private void PrintDistributionStatisticsAsRows(IEnumerable<IDataFrameColumn> dataFrameColumns)
    {
        var table = new Table().LeftAligned().Title("Table Schema Details");

        table.AddColumn("[u]Column[/]");
        table.AddColumn("[u]Data Type[/]");
        table.AddColumn("[u]Distribution[/]");
        table.AddColumn("[u]Mean[/]");
        table.AddColumn("[u]Mode[/]");
        table.AddColumn("[u]Q1[/]");
        table.AddColumn("[u]Median[/]");
        table.AddColumn("[u]Q3[/]");
        table.AddColumn("[u]Standard Deviation[/]");
        table.AddColumn("[u]Variance[/]");
        table.AddColumn("[u]Support Min[/]");
        table.AddColumn("[u]Support Max[/]");

        foreach (var dataFrameColumn in dataFrameColumns)
        {
            if (dataFrameColumn.Distribution is IUnivariateDistribution distribution)
            {
                table.AddRow(
                    $"[bold yellow]{dataFrameColumn.Name}[/]",
                    $"[bold yellow]{dataFrameColumn.Type}[/]",
                    distribution.ToString() ?? string.Empty,
                    distribution.Mean.ToString("F2"),
                    distribution.Mode.ToString("F2"),
                    distribution.Quartiles.Min.ToString("F2"),
                    distribution.Median.ToString("F2"),
                    distribution.Quartiles.Max.ToString("F2"),
                    Math.Sqrt(distribution.Variance).ToString("F2"),
                    distribution.Variance.ToString("F2"),
                    double.IsNegativeInfinity(distribution.Support.Min) ? "-infinity" : distribution.Support.Min.ToString("F2"),
                    double.IsPositiveInfinity(distribution.Support.Max) ? "infinity" : distribution.Support.Min.ToString("F2")
                );
            }

            if (dataFrameColumn is IMultivariateDistribution multivariateDistribution)
            {
                table.AddRow(
                    dataFrameColumn.Name,
                    dataFrameColumn.Type,
                    multivariateDistribution.ToString() ?? string.Empty,
                    string.Join(",", multivariateDistribution.Mean.Select(x => x.ToString("F2"))),
                    string.Join(",", multivariateDistribution.Mode.Select(x => x.ToString("F2"))),
                    string.Empty, // Q1
                    string.Join(",", multivariateDistribution.Median.Select(x => x.ToString("F2"))),
                    string.Empty, // Q2
                    string.Join(",", multivariateDistribution.Variance.Select(x => Math.Sqrt(x).ToString("F2"))),
                    string.Join(",", multivariateDistribution.Variance.Select(x => x.ToString("F2"))),
                    string.Empty, // Min,
                    string.Empty // Max,
                );
            }
        }

        _ansiConsole.Write(table);
    }

    private void PrintDistributionStatisticsAsColumns(IEnumerable<IDataFrameColumn> dataFrameColumns)
    {
        var table = new Table().LeftAligned().Title("Table Schema Details");

        // Add a column for the statistics
        table.AddColumn("[u]Statistic[/]");

        foreach (var dataFrameColumn in dataFrameColumns.Where(column => column.Distribution is IUnivariateDistribution))
            table.AddColumn($"[bold yellow]{dataFrameColumn.Name}[/]");

        // Populate the table with statistics
        var statisticsByType = GetStatistics(dataFrameColumns).ToList();

        foreach (var kvp in statisticsByType)
            table.AddRow(new List<string> { kvp.Key }.Concat(kvp.Value).ToArray());

        _ansiConsole.Write(table);
    }

    private Dictionary<string, List<string>> GetStatistics(IEnumerable<IDataFrameColumn> dataFrameColumns)
    {
        var statisticsByType = new Dictionary<string, List<string>>
        {
            { "Data Type", new List<string>() },
            { "Distribution", new List<string>() },
            { "Mean", new List<string>() },
            { "Mode", new List<string>() },
            { "Q1", new List<string>() },
            { "Median", new List<string>() },
            { "Q3", new List<string>() },
            { "Standard Deviation", new List<string>() },
            { "Variance", new List<string>() },
            { "Support Min", new List<string>() },
            { "Support Max", new List<string>() },
            { "Skewness", new List<string>() },
            { "Kurtosis", new List<string>() }
        };

        foreach (var dataFrameColumn in dataFrameColumns)
        {
            if (dataFrameColumn.Distribution is IUnivariateDistribution univariateDistribution)
            {
                statisticsByType["Data Type"].Add(dataFrameColumn.Type);
                statisticsByType["Distribution"].Add(univariateDistribution?.ToString() ?? string.Empty);
                statisticsByType["Mean"].Add(univariateDistribution!.Mean.ToString("F2"));
                statisticsByType["Mode"].Add(univariateDistribution.Mode.ToString("F2"));
                statisticsByType["Q1"].Add(univariateDistribution.Quartiles.Min.ToString("F2"));
                statisticsByType["Median"].Add(univariateDistribution.Median.ToString("F2"));
                statisticsByType["Q3"].Add(univariateDistribution.Quartiles.Max.ToString("F2"));
                statisticsByType["Standard Deviation"].Add(Math.Sqrt(univariateDistribution.Variance).ToString("F2"));
                statisticsByType["Variance"].Add(univariateDistribution.Variance.ToString("F2"));
                statisticsByType["Support Min"].Add(double.IsNegativeInfinity(univariateDistribution.Support.Min) ? "-infinity" : univariateDistribution.Support.Min.ToString("F2"));
                statisticsByType["Support Max"].Add(double.IsPositiveInfinity(univariateDistribution.Support.Max) ? "infinity" : univariateDistribution.Support.Min.ToString("F2"));
            }

            if (dataFrameColumn.Distribution is NormalDistribution normalDistribution)
            {
                statisticsByType["Skewness"].Add(normalDistribution.Skewness.ToString("F2"));
                statisticsByType["Kurtosis"].Add(normalDistribution.Kurtosis.ToString("F2"));
            }
            else
            {
                statisticsByType["Skewness"].Add(string.Empty);
                statisticsByType["Kurtosis"].Add(string.Empty);
            }
        }

        return statisticsByType;
    }

    private void PrintColumnStatisticsAsRows(Dictionary<string, ColumnStatistics> columns)
    {
        var table = new Table().LeftAligned().Title("CSV Data Statistics");

        table.AddColumn("[u]Column[/]");
        //table.AddColumn("[u]Data Type[/]");
        //table.AddColumn("[u]Distribution[/]");
        table.AddColumn("[u]Mean[/]");
        table.AddColumn("[u]Mode[/]");
        table.AddColumn("[u]Q1[/]");
        table.AddColumn("[u]Median[/]");
        table.AddColumn("[u]Q3[/]");
        table.AddColumn("[u]Standard Deviation[/]");
        table.AddColumn("[u]Variance[/]");
        table.AddColumn("[u]Support Min[/]");
        table.AddColumn("[u]Support Max[/]");

        foreach (var column in columns)
        {
            table.AddRow(
                $"[bold yellow]{column.Key}[/]",
                //$"[bold yellow]{dataFrameColumn.Type}[/]",
                //distribution.ToString() ?? string.Empty,
                column.Value.Mean.ToString("F2"),
                column.Value.Mode.ToString("F2"),
                column.Value.Q1.ToString("F2"),
                column.Value.Median.ToString("F2"),
                column.Value.Q3.ToString("F2"),
                column.Value.StandardDeviation.ToString("F2"),
                column.Value.Variance.ToString("F2"),
                column.Value.Minimum.ToString("F2"),
                column.Value.Maximum.ToString("F2")
            );
        }

        _ansiConsole.Write(table);
    }

    private void PrintColumnStatisticsAsColumns(Dictionary<string, ColumnStatistics> columns)
    {
        var table = new Table().LeftAligned().Title("Table Schema Details");

        // Add a column for the statistics
        table.AddColumn("[u]Statistic[/]");

        foreach (var column in columns)
            table.AddColumn($"[bold yellow]{column.Key}[/]");

        // Populate the table with statistics
        var statisticsByType = GetColumnStatistics(columns).ToList();

        foreach (var kvp in statisticsByType)
            table.AddRow(new List<string> { kvp.Key }.Concat(kvp.Value).ToArray());

        _ansiConsole.Write(table);
    }

    private Dictionary<string, List<string>> GetColumnStatistics(Dictionary<string, ColumnStatistics> columns)
    {
        var statisticsByType = new Dictionary<string, List<string>>
        {
            //{ "Data Type", new List<string>() },
            //{ "Distribution", new List<string>() },
            { "Mean", new List<string>() },
            { "Mode", new List<string>() },
            { "Q1", new List<string>() },
            { "Median", new List<string>() },
            { "Q3", new List<string>() },
            { "Standard Deviation", new List<string>() },
            { "Variance", new List<string>() },
            { "Min", new List<string>() },
            { "Max", new List<string>() },
            //{ "Skewness", new List<string>() },
            //{ "Kurtosis", new List<string>() }
        };

        foreach (var column in columns)
        {
            //statisticsByType["Data Type"].Add(dataFrameColumn.Type);
            //statisticsByType["Distribution"].Add(univariateDistribution?.ToString() ?? string.Empty);
            statisticsByType["Mean"].Add(column.Value!.Mean.ToString("F2"));
            statisticsByType["Mode"].Add(column.Value.Mode.ToString("F2"));
            statisticsByType["Q1"].Add(column.Value.Q1.ToString("F2"));
            statisticsByType["Median"].Add(column.Value.Median.ToString("F2"));
            statisticsByType["Q3"].Add(column.Value.Q3.ToString("F2"));
            statisticsByType["Standard Deviation"].Add(column.Value.StandardDeviation.ToString("F2"));
            statisticsByType["Variance"].Add(column.Value.Variance.ToString("F2"));
            statisticsByType["Min"].Add(column.Value.Minimum.ToString("F2"));
            statisticsByType["Max"].Add(column.Value.Maximum.ToString("F2"));

            //if (dataFrameColumn.Distribution is NormalDistribution normalDistribution)
            //{
            //    statisticsByType["Skewness"].Add(normalDistribution.Skewness.ToString("F2"));
            //    statisticsByType["Kurtosis"].Add(normalDistribution.Kurtosis.ToString("F2"));
            //}
            //else
            //{
            //    statisticsByType["Skewness"].Add(string.Empty);
            //    statisticsByType["Kurtosis"].Add(string.Empty);
            //}
        }

        return statisticsByType;
    }
}
