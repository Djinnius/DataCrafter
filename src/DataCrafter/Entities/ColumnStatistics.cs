namespace DataCrafter.Commands.DataFrame.CsvStatistics;
internal sealed class ColumnStatistics
{
    private double[] _values = [];
    private double? _mean;
    private double? _mode;
    private double? _q1;
    private double? _median;
    private double? _q3;
    private double? _standardDeviation;
    private double? _variance;
    private double? _minimum;
    private double? _maximum;
    private int _count;

    public ColumnStatistics(int count = default)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(count);

        if (count > default(int))
            _values = new double[count];
    }

    public void Add(double value)
    {
        if (_count == _values.Length)
        {
            Array.Resize(ref _values, _count == 0 ? 4 : _count * 2);
        }

        _values[_count++] = value;
    }
    public double[] Values { get { return _values; } }
    public double Mean => _mean ??= Accord.Statistics.Measures.Mean(_values);
    public double Mode => _mode ??= Accord.Statistics.Measures.Mode(_values);
    public double Q1 => _q1 ??= GetQ1();
    public double Median => _median ??= GetMedian();
    public double Q3 => _q3 ??= GetQ3();
    public double StandardDeviation => _standardDeviation ??= Accord.Statistics.Measures.StandardDeviation(_values);
    public double Variance => _variance ??= Accord.Statistics.Measures.Variance(_values);
    public double Minimum => _minimum ??= _values.Min();
    public double Maximum => _maximum ??= _values.Max();

    private double GetQ1()
    {
        _median = Accord.Statistics.Measures.Quartiles(_values, out var q1, out double q3);
        _q1 = q1;
        _q3 = q3;

        return q1;
    }

    private double GetQ3()
    {
        _median = Accord.Statistics.Measures.Quartiles(_values, out var q1, out double q3);
        _q1 = q1;
        _q3 = q3;

        return q3;
    }

    private double GetMedian()
    {
        _median = Accord.Statistics.Measures.Quartiles(_values, out var q1, out double q3);
        _q1 = q1;
        _q3 = q3;

        return _median.HasValue ? _median.Value : 0;
    }
}
