using DataCrafter.Entities;

namespace DataCrafter.Services.Repositories;
internal interface ITableSchemaRepository
{
    void DeleteDataFrameColumn(string name);
    IDataFrameColumn? GetDataFrameColumnByName(string name);
    IEnumerable<IDataFrameColumn> GetAllDataFrameColumns();
    void UpsertDataFrameColumn(IDataFrameColumn dataFrameColumn);
    void SetToDefaultData();
    void DeleteAllDataFrameColumns();
    void OverwriteAllDataFrameColumns(IEnumerable<IDataFrameColumn> dataFrameColumns);
}
