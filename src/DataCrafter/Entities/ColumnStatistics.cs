namespace DataCrafter.Entities;
internal sealed class ColumnStatistics
{
    private readonly List<double> _valuesList = new List<double>();
    private double[] _valuesArray = [];
    private double? _mean;
    private double? _mode;
    private double? _q1;
    private double? _median;
    private double? _q3;
    private double? _standardDeviation;
    private double? _variance;
    private double? _minimum;
    private double? _maximum;
    private double? _skewness;
    private double? _kurtosis;

    public ColumnStatistics(int count = default)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(count);

        if (count > default(int))
            _valuesArray = new double[count];
    }

    public void Add(double value)
        => _valuesList.Add(value);

    public List<double> Values => _valuesList;
    public double[] ValuesArray => GetValuesArray();
    public double Mean => _mean ??= Accord.Statistics.Measures.Mean(GetValuesArray());
    public double Mode => _mode ??= Accord.Statistics.Measures.Mode(GetValuesArray());
    public double Q1 => _q1 ??= GetQ1();
    public double Median => _median ??= GetMedian();
    public double Q3 => _q3 ??= GetQ3();
    public double StandardDeviation => _standardDeviation ??= Accord.Statistics.Measures.StandardDeviation(GetValuesArray());
    public double Variance => _variance ??= Accord.Statistics.Measures.Variance(GetValuesArray());
    public double Minimum => _minimum ??= GetValuesArray().Min();
    public double Maximum => _maximum ??= GetValuesArray().Max();
    public double Skewness => _skewness ??= Accord.Statistics.Measures.Skewness(GetValuesArray());
    public double Kurtosis => _kurtosis ??= Accord.Statistics.Measures.Kurtosis(GetValuesArray());

    private double[] GetValuesArray()
    {
        if (_valuesArray.Count() != _valuesList.Count)
            _valuesArray = _valuesList.ToArray();

        return _valuesArray;
    }


    private double GetQ1()
    {
        _median = Accord.Statistics.Measures.Quartiles(GetValuesArray(), out var q1, out var q3);
        _q1 = q1;
        _q3 = q3;

        return q1;
    }

    private double GetQ3()
    {
        _median = Accord.Statistics.Measures.Quartiles(GetValuesArray(), out var q1, out var q3);
        _q1 = q1;
        _q3 = q3;

        return q3;
    }

    private double GetMedian()
    {
        _median = Accord.Statistics.Measures.Quartiles(GetValuesArray(), out var q1, out var q3);
        _q1 = q1;
        _q3 = q3;

        return _median.HasValue ? _median.Value : 0;
    }
}
