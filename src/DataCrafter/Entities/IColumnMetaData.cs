namespace DataCrafter.Entities;
internal interface IColumnMetaData
{
    /// <summary>
    ///     Gets the name of the column.
    /// </summary>
    string Name { get; }

    /// <summary>
    ///     Gets the type of data, e.g. double, int etc.
    /// </summary>
    string DataType { get; }
}
