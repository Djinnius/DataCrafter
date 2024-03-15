﻿using Accord.Statistics.Distributions;
using DataCrafter.Commands.DataFrame.CsvStatistics;
using DataCrafter.Entities;

namespace DataCrafter.Services.ConsoleWriters;
internal interface IDistributionPlotterConsoleWriter
{
    void PlotPdf(IUnivariateDistribution univariateDistribution, double percentile = 0.9545, int numPoints = 20, string name = "");
    void PlotPdf(IUnivariateDistribution distribution, double lowerBound, double upperBound, int numPoints = 20, string name = "");
    void PrintDistributionsToConsole(IEnumerable<IDataFrameColumn> dataFrameColumns);
    void PlotHistogram(ColumnStatistics columnStatistics, int numPoints = 20, string name = "");
}
