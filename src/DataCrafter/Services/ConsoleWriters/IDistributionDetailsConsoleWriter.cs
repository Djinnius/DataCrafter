using Accord.Statistics.Distributions;

namespace DataCrafter.Services.ConsoleWriters;
internal interface IDistributionDetailsConsoleWriter
{
    void WriteDistributionMetaDataToConsole(IEnumerable<IDistribution> distributions,
                                            DistributionInfoType colourBy = DistributionInfoType.Name,
                                            DistributionInfoType orderBy = DistributionInfoType.Name,
                                            bool orderByDescending = false);
}
