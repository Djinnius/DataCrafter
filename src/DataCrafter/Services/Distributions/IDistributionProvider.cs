using Accord.Statistics.Distributions;

namespace DataCrafter.Services.Distributions;
internal interface IDistributionProvider
{
    IList<IUnivariateDistribution> GetUnivariateDistributions();
    //IList<IUnivariateDistribution> Temp();
}
