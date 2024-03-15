using System.Text.Json.Serialization;
using Accord.Statistics.Distributions;
using DataCrafter.DistributionConfigurations;

namespace DataCrafter.Entities;

[JsonDerivedType(typeof(DataFrameColumn), "dataFrameColumn")]
internal interface IDataFrameColumn
{
    /// <summary>
    ///     Gets the name of the column.
    /// </summary>
    string Name { get; }

    /// <summary>
    ///     Gets the type of data to generate, e.g. double, int etc.
    /// </summary>
    string Type { get; }

    /// <summary>
    ///     Gets the distribution configuration used to generate data.
    /// </summary>
    IDistributionConfig DistributionConfig { get; }

    /// <summary>
    ///     Gets the sampleable distribution.
    /// </summary>
    [JsonIgnore]
    ISampleableDistribution<double> Distribution { get; }

    /// <summary>
    ///     Gets the seed to use when generating pseudo-random data. Zero indicates to generate from random seed.
    /// </summary>
    int Seed { get; }

    /// <summary>
    ///     Gets the ordinal of the column. Used to order the columns.
    /// </summary>
    int Ordinal { get; set; }
}
