namespace DataCrafter.Entities;

internal interface IColumnStatistics
{
    double Kurtosis { get; }
    double Maximum { get; }
    double Mean { get; }
    double Median { get; }
    double Minimum { get; }
    double Mode { get; }
    double Q1 { get; }
    double Q3 { get; }
    double Skewness { get; }
    double StandardDeviation { get; }
    double Variance { get; }
}