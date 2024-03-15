
using Accord.Statistics.Distributions;

namespace DataCrafter.Services.Distributions;

internal interface IDistributionInfoService
{
    ExtendedDistributionInfo GetDistributionProperties(IDistribution distribution);
}
