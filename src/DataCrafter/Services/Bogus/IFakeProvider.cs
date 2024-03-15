using Bogus;
using DataCrafter.Entities;

namespace DataCrafter.Services.Bogus;

/// <summary>
///     Represents a service that provides methods for generating fake data based on data frame columns.
/// </summary>
internal interface IFakeProvider
{
    /// <summary>
    ///     Builds a Faker instance for generating data records based on the provided <paramref name="dataFrameColumns"/>.
    /// </summary>
    /// <param name="dataFrameColumns"> The collection of data frame columns providing information about the data structure. </param>
    /// <returns> A Faker instance configured for generating data records. </returns>
    Faker<DynamicClass> BuildDynamicFaker(IList<IDataFrameColumn> dataFrameColumns);
}
