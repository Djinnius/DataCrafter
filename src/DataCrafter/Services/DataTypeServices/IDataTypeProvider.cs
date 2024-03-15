
namespace DataCrafter.Services.DataTypeServices;

internal interface IDataTypeProvider
{
    HashSet<string> DoubleAliases { get; }
    HashSet<string> IntAliases { get; }
}