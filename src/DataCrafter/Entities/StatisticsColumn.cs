namespace DataCrafter.Entities;
internal class StatisticsColumn : IColumnStatistics, IColumnMetaData
{
    public string Name { get; set; } = string.Empty;

    public string DataType { get; set; } = string.Empty;

    public double Kurtosis { get; set; }

    public double Maximum { get; set; }

    public double Mean { get; set; }

    public double Median { get; set; }

    public double Minimum { get; set; }

    public double Mode { get; set; }

    public double Q1 { get; set; }

    public double Q3 { get; set; }

    public double Skewness { get; set; }

    public double StandardDeviation { get; set; }

    public double Variance { get; set; }
}
