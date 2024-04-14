using Spectre.Console;

namespace DataCrafter.Services.Spectre;

/// <inheritdoc cref={ITableProvider}/>
internal sealed class TableProvider : ITableProvider
{
    public Table GetTable(string title) => new Table().LeftAligned().Border(TableBorder.Horizontal).Title(title);
}
