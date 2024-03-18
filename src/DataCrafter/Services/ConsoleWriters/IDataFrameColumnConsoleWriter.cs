using DataCrafter.Entities;

namespace DataCrafter.Services.ConsoleWriters;
internal interface IDataFrameColumnConsoleWriter
{
    void PrintColumnsToConsole(IEnumerable<IDataFrameColumn> dataFrameColumns, bool columnsAsRows = false);
    void PrintColumnsToConsole(Dictionary<string, ColumnStatistics> columns, bool columnsAsRows = false);
}
