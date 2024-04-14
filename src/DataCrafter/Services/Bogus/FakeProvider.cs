using Bogus;
using DataCrafter.Entities;
using DataCrafter.Services.DataTypeServices;

namespace DataCrafter.Services.Bogus;

/// <inheritdoc cref="IFakeProvider"/>
internal sealed class FakeProvider : IFakeProvider
{
    private readonly IDataTypeProvider _dataTypeProvider;

    public FakeProvider(IDataTypeProvider dataTypeProvider)
    {
        _dataTypeProvider = dataTypeProvider;
    }

    public Faker<DynamicClass> BuildDynamicFaker(IList<IDataFrameColumn> dataFrameColumns)
    {
        long id = 1;

        var faker = new Faker<DynamicClass>()
            .StrictMode(false)
            .RuleFor(x => x.Id, _ => id++)
            .RuleFor(x => x.Attributes, y =>
            {
                var dynamicObject = new Dictionary<string, object>();

                foreach (var dataFrameColumn in dataFrameColumns)
                {
                    if (_dataTypeProvider.IntAliases.Contains(dataFrameColumn.DataType))
                        dynamicObject.Add(dataFrameColumn.Name, Math.Round(GenerateDistributionValue(dataFrameColumn)));

                    if (_dataTypeProvider.DoubleAliases.Contains(dataFrameColumn.DataType))
                        dynamicObject.Add(dataFrameColumn.Name, GenerateDistributionValue(dataFrameColumn));
                }

                return dynamicObject;
            });

        return faker;
    }

    //public Faker<DynamicClass> BuildDynamicFaker(IList<IDataFrameColumn> dataFrameColumns)
    //{
    //    // Max 2,147,483,647 rows
    //    var id = 1;

    //    var faker = new Faker<DynamicClass>()
    //        .StrictMode(false)
    //        .RuleFor(x => x.Id, _ => id++)
    //        .RuleFor(x => x.Attributes, y =>
    //        {
    //            var dynamicObject = new ExpandoObject() as IDictionary<string, object>;

    //            foreach (var dataFrameColumn in dataFrameColumns)
    //            {
    //                if (dataFrameColumn.Type == "system.int")
    //                    dynamicObject.Add(dataFrameColumn.Name, Math.Round(GenerateDistributionValue(dataFrameColumn)));

    //                if (dataFrameColumn.Type == "system.double")
    //                    dynamicObject.Add(dataFrameColumn.Name, GenerateDistributionValue(dataFrameColumn));
    //            }

    //            return dynamicObject;
    //        });

    //    return faker;
    //}

    private static double GenerateDistributionValue(IDataFrameColumn dataFrameColumn)
        => dataFrameColumn.Seed is default(int)
        ? dataFrameColumn.Distribution.Generate()
        : dataFrameColumn.Distribution.Generate(dataFrameColumn.Seed);
}
