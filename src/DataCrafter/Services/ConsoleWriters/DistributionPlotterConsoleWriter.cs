using Accord.Statistics.Distributions;
using DataCrafter.Entities;
using DataCrafter.Services.Mappers;
using Spectre.Console;

namespace DataCrafter.Services.ConsoleWriters;

internal sealed class DistributionPlotterConsoleWriter : IDistributionPlotterConsoleWriter
{
    private readonly IDistributionColorMapper _distributionColorMapper;
    private readonly IAnsiConsole _ansiConsole;

    public DistributionPlotterConsoleWriter(IDistributionColorMapper distributionColorMapper, IAnsiConsole ansiConsole)
    {
        _distributionColorMapper = distributionColorMapper;
        _ansiConsole = ansiConsole;
    }

    public void PrintDistributionsToConsole(IEnumerable<IDataFrameColumn> dataFrameColumns)
    {
        _ansiConsole.WriteLine();
        _ansiConsole.Write(new Rule($"[u]Distribution Details ({dataFrameColumns.Count()} Distributions)[/]").RuleStyle("grey").Centered());
        _ansiConsole.WriteLine();

        foreach (var dataFrameColumn in dataFrameColumns)
        {
            if (dataFrameColumn.Distribution is IUnivariateDistribution univariateDistribution)
                PlotPdf(univariateDistribution, name: dataFrameColumn.DistributionConfig.DistributionType);
        }

        _ansiConsole.WriteLine();
    }

    public void PlotPdf(IUnivariateDistribution univariateDistribution, double percentile = 0.9545, int numPoints = 20, string name = "")
    {
        var range = univariateDistribution.GetRange(percentile); // default 2 standard deviations on a normal distribution
        PlotPdf(univariateDistribution, range.Min, range.Max, numPoints, name);
        _ansiConsole.WriteLine();
    }

    public void PlotPdf(IUnivariateDistribution distribution, double lowerBound, double upperBound, int numPoints = 20, string name = "")
    {
        var chart = new BarChart()
            .Width(80)
            .Label($"{name} {distribution} Probability Density Function (PDF)");

        // Calculate PDF values for the specified range
        var step = (upperBound - lowerBound) / (numPoints - 1);
        var values = new List<(double x, double y)>();

        for (var i = 0; i < numPoints; i++)
        {
            var x = lowerBound + i * step;
            var y = distribution.ProbabilityFunction(x);
            values.Add((x, y));
        }

        chart.AddItems(values, value => new BarChartItem(value.x.ToString("N2"), Math.Round(value.y, 3), _distributionColorMapper.GetColorForDistribution(distribution)));
        _ansiConsole.Write(chart);
    }

    public void PlotHistogram(DataColumn columnStatistics, int numPoints = 20, string name = "")
    {
        var chart = new BarChart()
            .Width(80)
            .Label($"{name} Histogram");

        var min = Math.Floor(columnStatistics.Minimum);
        var max = Math.Ceiling(columnStatistics.Maximum);
        var step = (max - min) / (numPoints - 1);
        var buckets = Bucketise(columnStatistics.Values, min, max, numPoints);
        var values = new List<(double x, double y)>();

        for (var i = 0; i < numPoints; i++)
        {
            var x = min + i * step;
            var y = buckets[i];
            values.Add((x, y));
        }

        chart.AddItems(values, value => new BarChartItem(value.x.ToString("N2"), Math.Round(value.y, 3), Color.LightSkyBlue1));
        _ansiConsole.Write(chart);
    }

    public void PlotHistogramWithOutliers(DataColumn columnStatistics, double maxOutlerBelowMean, double minOutlierAboveMean, int numPoints = 40, string name = "")
    {
        var chart = new BarChart()
            .Width(80)
            .Label($"{name} Histogram");

        var min = Math.Floor(columnStatistics.Minimum);
        var max = Math.Ceiling(columnStatistics.Maximum);
        var step = (max - min) / (numPoints - 1);
        var buckets = Bucketise(columnStatistics.Values, min, max, numPoints);
        var values = new List<(double x, double y)>();

        for (var i = 0; i < numPoints; i++)
        {
            var x = min + i * step;
            var y = buckets[i];
            values.Add((x, y));
        }

        var colorFunc = ((double x, double y) val) => val.x <= maxOutlerBelowMean ? Color.LightSkyBlue1 : val.x >= minOutlierAboveMean ? Color.LightPink1 : Color.LightGreen;
        chart.AddItems(values, value => new BarChartItem(value.x.ToString("N2"), Math.Round(value.y, 3), colorFunc(value)));
        _ansiConsole.Write(chart);
    }

    private enum BucketizeDirection
    {
        LowerBoundInclusive,
        UpperBoundInclusive
    }

    private int[] Bucketise(IEnumerable<double> source, double min, double max, int totalBuckets, BucketizeDirection inclusivity = BucketizeDirection.UpperBoundInclusive)
    {
        var buckets = new int[totalBuckets];
        var bucketSize = (max - min) / totalBuckets;

        if (inclusivity == BucketizeDirection.LowerBoundInclusive)
        {
            foreach (var value in source)
            {
                int bucketIndex = (int)((value - min) / bucketSize);
                if (bucketIndex == totalBuckets)
                    continue;
                buckets[bucketIndex]++;
            }
        }
        else
        {
            foreach (var value in source)
            {
                int bucketIndex = (int)Math.Ceiling((value - min) / bucketSize) - 1;
                if (bucketIndex < 0)
                    continue;
                buckets[bucketIndex]++;
            }
        }

        return buckets;
    }
}
