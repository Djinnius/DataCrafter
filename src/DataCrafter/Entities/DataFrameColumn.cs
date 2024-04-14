using System.Text.Json.Serialization;
using Accord.Statistics.Distributions;
using Accord.Statistics.Distributions.Univariate;
using DataCrafter.Pocos.DistributionConfigurations;

namespace DataCrafter.Entities;

internal class DataFrameColumn : IDataFrameColumn
{
    private ISampleableDistribution<double>? _distribution;

    public string Name { get; set; } = string.Empty;

    public string DataType { get; set; } = string.Empty;

    public IDistributionConfig DistributionConfig { get; set; } = new NormalDistributionConfig { Mean = 5, StandardDeviation = 1 };

    public int Seed { get; set; }

    public int Ordinal { get; set; }

    [JsonIgnore]
    public ISampleableDistribution<double> Distribution
    {
        get
        {
            if (_distribution == null)
            {
                // Initialize distribution using DistributionConfig
                _distribution = InitializeDistribution();
            }

            return _distribution;
        }
    }

    private ISampleableDistribution<double> InitializeDistribution()
    {
        switch (DistributionConfig)
        {
            case CauchyDistributionConfig cauchyConfig:
                return new CauchyDistribution(cauchyConfig.Location, cauchyConfig.Scale);
            case NormalDistributionConfig normalConfig:
                return new NormalDistribution(normalConfig.Mean, normalConfig.StandardDeviation);
            case ExponentialDistributionConfig exponentialConfig:
                return new ExponentialDistribution(exponentialConfig.Rate);
            case GammaDistributionConfig gammaConfig:
                return new GammaDistribution(gammaConfig.Shape, gammaConfig.Scale);
            case LogNormalDistributionConfig logNormalConfig:
                return new LognormalDistribution(logNormalConfig.Mean, logNormalConfig.StandardDeviation);
            case UniformDistributionConfig uniformConfig:
                return new UniformContinuousDistribution(uniformConfig.Min, uniformConfig.Max);
            case ChiSquareDistributionConfig chiSquareConfig:
                return new ChiSquareDistribution(chiSquareConfig.DegreesOfFreedom);
            case LaplaceDistributionConfig laplaceConfig:
                return new LaplaceDistribution(laplaceConfig.Location, laplaceConfig.Scale);
            case LogisticDistributionConfig logisticConfig:
                return new LogisticDistribution(logisticConfig.Location, logisticConfig.Scale);
            case TriangularDistributionConfig triangularConfig:
                return new TriangularDistribution(triangularConfig.Minimum, triangularConfig.Maximum, triangularConfig.Mode);
            case WeibullDistributionConfig weibullConfig:
                return new WeibullDistribution(weibullConfig.Shape, weibullConfig.Scale);
        }

        // Default case or throw an exception for unsupported types
        throw new NotSupportedException("Unsupported distribution type");
    }
}
