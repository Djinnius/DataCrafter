namespace DataCrafter.Services.DataTypeServices;
internal sealed class DataTypeProvider : IDataTypeProvider
{
    public HashSet<string> DoubleAliases { get; } = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
    {
        "double", "d", "system.double"
    };

    public HashSet<string> IntAliases { get; } = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
    {
        "int", "i", "system.int"
    };
}
