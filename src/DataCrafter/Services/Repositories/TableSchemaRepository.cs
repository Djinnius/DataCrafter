using DataCrafter.Entities;
using DataCrafter.Options;
using DataCrafter.Pocos.DistributionConfigurations;
using Microsoft.Extensions.Options;
using Spectre.Console;
using System.Text.Json;

namespace DataCrafter.Services.Repositories;

/// <summary>
///     DataFrameColumn collection repository. Stores dataFrameColumn configurations in a json file
///     that can be written/read across commands.
/// </summary>
internal sealed class TableSchemaRepository : ITableSchemaRepository
{
    private static string _dataFile => Path.Combine(GetDataCrafterDirectory(), "columns.json");

    private IList<IDataFrameColumn> _dataFrameColumns = new List<IDataFrameColumn>();
    private readonly DataCrafterOptions _options;

    public TableSchemaRepository(IOptions<DataCrafterOptions> options)
    {
        if (File.Exists(_dataFile))
        {
            var json = File.ReadAllText(_dataFile);
            _dataFrameColumns = JsonSerializer.Deserialize<IList<IDataFrameColumn>>(json) ?? new List<IDataFrameColumn>();
        }

        _options = options.Value;
    }

    public void UpsertDataFrameColumn(IDataFrameColumn dataFrameColumn)
    {
        if (_dataFrameColumns.Any(x => x.Name == dataFrameColumn.Name))
            DeleteDataFrameColumn(dataFrameColumn.Name);

        _dataFrameColumns.Add(dataFrameColumn!);
        SaveRepository();
    }

    public void DeleteDataFrameColumn(string name)
    {
        if (!_dataFrameColumns.Any(x => x.Name == name))
            throw new ArgumentException("DataFrameColumn does not exist in the repository.", nameof(name));

        var remove = GetDataFrameColumnByName(name);
        _dataFrameColumns.Remove(remove!);
        SaveRepository();
    }

    public IDataFrameColumn? GetDataFrameColumnByName(string name) =>
        _dataFrameColumns.FirstOrDefault(x => x.Name == name);

    public IEnumerable<IDataFrameColumn> GetAllDataFrameColumns()
        => _dataFrameColumns.OrderBy(x => x.Ordinal);

    public void DeleteAllDataFrameColumns()
    {
        _dataFrameColumns.Clear();
        SaveRepository();
    }

    public void OverwriteAllDataFrameColumns(IEnumerable<IDataFrameColumn> dataFrameColumns)
    {
        _dataFrameColumns = new List<IDataFrameColumn>(dataFrameColumns);
        SaveRepository();
    }

    public void SetToDefaultData()
    {
        WriteDefaultData();
        SaveRepository();
    }

    private void WriteDefaultData()
    {
        var random = new Random(_options.Seed);

        _dataFrameColumns = new List<IDataFrameColumn>
        {
            new DataFrameColumn
            {
                Name = "Cauchy",
                Type = "double",
                DistributionConfig = new CauchyDistributionConfig { Location = 5, Scale = 2 },
                Seed = _options.IsDeterministic ? random.Next(int.MinValue, int.MaxValue) : 0
            },
            new DataFrameColumn
            {
                Name = "Normal",
                Type = "double",
                DistributionConfig = new NormalDistributionConfig { Mean = 0, StandardDeviation = 1 },
                Seed = _options.IsDeterministic ? random.Next(int.MinValue, int.MaxValue) : 0
            },
            new DataFrameColumn
            {
                Name = "Exponential",
                Type = "double",
                DistributionConfig = new ExponentialDistributionConfig { Rate = 0.5 },
                Seed = _options.IsDeterministic ? random.Next(int.MinValue, int.MaxValue) : 0
            },
            new DataFrameColumn
            {
                Name = "Gamma",
                Type = "double",
                DistributionConfig = new GammaDistributionConfig { Shape = 2, Scale = 0.5 },
                Seed = _options.IsDeterministic ? random.Next(int.MinValue, int.MaxValue) : 0
            },
            new DataFrameColumn
            {
                Name = "LogNormal",
                Type = "double",
                DistributionConfig = new LogNormalDistributionConfig { Mean = 0, StandardDeviation = 1 },
                Seed = _options.IsDeterministic ? random.Next(int.MinValue, int.MaxValue) : 0
            },
            new DataFrameColumn
            {
                Name = "Uniform",
                Type = "double",
                DistributionConfig = new UniformDistributionConfig { Min = 0, Max = 1 },
                Seed = _options.IsDeterministic ? random.Next(int.MinValue, int.MaxValue) : 0
            }
        };
    }

    private void SaveRepository()
    {
        var data = JsonSerializer.Serialize(_dataFrameColumns);
        AnsiConsole.WriteLine(_dataFile);
        File.WriteAllText(_dataFile, data);
    }

    private static string GetDataCrafterDirectory()
    {
        var dataCrafterDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "DataCrafter");

        // Create the directory if it doesn't exist
        if (!Directory.Exists(dataCrafterDirectory))
            Directory.CreateDirectory(dataCrafterDirectory);

        return dataCrafterDirectory;
    }

}
