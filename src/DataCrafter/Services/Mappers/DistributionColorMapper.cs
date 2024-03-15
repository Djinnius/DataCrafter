using Accord.Statistics.Distributions;
using Accord.Statistics.Distributions.Univariate;
using Spectre.Console;

namespace DataCrafter.Services.Mappers;

public class DistributionColorMapper : IDistributionColorMapper
{
    private readonly Dictionary<Type, Color> _distributionColors;

    public DistributionColorMapper()
    {
        _distributionColors = new Dictionary<Type, Color>
        {
            { typeof(NormalDistribution), Color.LightSkyBlue3_1 },
            { typeof(ExponentialDistribution), Color.Green1 },
            { typeof(LogisticDistribution), Color.Red1 },
            { typeof(ChiSquareDistribution), Color.Orange1 },
            { typeof(CauchyDistribution), Color.Cyan1 },
            { typeof(LaplaceDistribution), Color.Magenta1 },
            { typeof(TriangularDistribution), Color.Yellow1 },
            { typeof(UniformContinuousDistribution), Color.Purple },
            { typeof(WeibullDistribution), Color.Teal },
            { typeof(BetaDistribution), Color.Orange3 },
            { typeof(GammaDistribution), Color.Pink1 },
            { typeof(GumbelDistribution), Color.Cyan3 }
        };
    }

    public Color GetColorForDistribution(IUnivariateDistribution distribution)
    {
        var distributionType = distribution.GetType();

        if (_distributionColors.TryGetValue(distributionType, out var color))
            return color;

        return Color.Grey;
    }
}
