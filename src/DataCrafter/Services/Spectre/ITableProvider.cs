using Spectre.Console;

namespace DataCrafter.Services.Spectre;

/// <summary>
///     Represents a provider for tables.
/// </summary>
internal interface ITableProvider
{
    /// <summary>
    ///     Gets a table with the specified title.
    /// </summary>
    /// <param name="title"> The title of the table. </param>
    /// <returns> A new table with the specified <paramref name="title"/>. </returns>
    Table GetTable(string title);
}
