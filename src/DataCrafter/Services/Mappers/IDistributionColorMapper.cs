using Accord.Statistics.Distributions;
using Spectre.Console;

namespace DataCrafter.Services.Mappers;
public interface IDistributionColorMapper
{
    Color GetColorForDistribution(IUnivariateDistribution distribution);
}