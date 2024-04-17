using Accord.Statistics.Distributions;
using DataCrafter.Entities;

namespace DataCrafter.Services.ConsoleWriters;
internal interface IDistributionPlotterConsoleWriter
{
    void PlotPdf(IUnivariateDistribution univariateDistribution, double percentile = 0.9545, int numPoints = 20, string name = "");
    void PlotPdf(IUnivariateDistribution distribution, double lowerBound, double upperBound, int numPoints = 20, string name = "");
    void PrintDistributionsToConsole(IEnumerable<IDataFrameColumn> dataFrameColumns);
    void PlotHistogram(DataColumn columnStatistics, int numPoints = 20, string name = "");
    void PlotHistogramWithOutliers(DataColumn columnStatistics, double maxOutlerBelowMean, double minOutlierAboveMean, int numPoints = 40, string name = "");
}
