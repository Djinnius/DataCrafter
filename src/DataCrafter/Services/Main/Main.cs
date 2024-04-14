using DataCrafter.Commands.DataFrame.ShapiroWilk;
using DataCrafter.Commands.Distributions.Details;
using DataCrafter.Entities;
using DataCrafter.Options;
using DataCrafter.Options.WritableOptions;
using DataCrafter.Pocos.DistributionConfigurations;
using DataCrafter.Services.Bogus;
using DataCrafter.Services.ConsoleWriters;
using DataCrafter.Services.Distributions;
using DataCrafter.Services.FileIO;
using Microsoft.Extensions.Options;
using Spectre.Console;

namespace DataCrafter.Services.Main;
internal class Main
{
    private readonly IFakeProvider _fakeProvider;
    private readonly IDataFrameColumnConsoleWriter _dataFrameColumnConsoleWriter;
    private readonly IGeneratedDataConsoleWriter _generatedDataConsoleSink;
    private readonly IDistributionProvider _distributionProvider;
    private readonly IDistributionDetailsConsoleWriter _distributionFittingConsoleWriter;
    private readonly IDataFrameCsvSink _dataFrameCsvSink;
    private readonly IDistributionPlotterConsoleWriter _distributionPlotterConsoleWriter;
    private readonly IWritableOptions<ApiKeysOptions> _apiKeyOptions;
    private readonly DataCrafterOptions _options;

    public Main(
        IFakeProvider fakeProvider,
        IDataFrameColumnConsoleWriter dataFrameColumnConsoleWriter,
        IGeneratedDataConsoleWriter generatedDataConsoleSink,
        IDistributionProvider distributionProvider,
        IDistributionDetailsConsoleWriter distributionFittingConsoleWriter,
        IDataFrameCsvSink dataFrameCsvSink,
        IDistributionPlotterConsoleWriter distributionPlotterConsoleWriter,
        IOptions<DataCrafterOptions> options,
        IWritableOptions<ApiKeysOptions> apiKeyOptions)
    {
        _fakeProvider = fakeProvider;
        _dataFrameColumnConsoleWriter = dataFrameColumnConsoleWriter;
        _generatedDataConsoleSink = generatedDataConsoleSink;
        _distributionProvider = distributionProvider;
        _distributionFittingConsoleWriter = distributionFittingConsoleWriter;
        _dataFrameCsvSink = dataFrameCsvSink;
        _distributionPlotterConsoleWriter = distributionPlotterConsoleWriter;
        _apiKeyOptions = apiKeyOptions;
        _options = options.Value;
    }

    public async Task Execute()
    {
        await Task.CompletedTask;
        var random = new Random(_options.Seed);

        IList<IDataFrameColumn> columns = new List<IDataFrameColumn>
        {
            new DataFrameColumn
            {
                Name = "Cauchy",
                DataType = "double",
                DistributionConfig = new CauchyDistributionConfig { Location = 10, Scale = 3 },
                Seed = _options.IsDeterministic ? random.Next(int.MinValue, int.MaxValue) : 0
            },
            new DataFrameColumn
            {
                Name = "Normal",
                DataType = "double",
                DistributionConfig = new NormalDistributionConfig { Mean = 5, StandardDeviation = 3 },
                Seed = _options.IsDeterministic ? random.Next(int.MinValue, int.MaxValue) : 0
            },
            new DataFrameColumn
            {
                Name = "Exponential",
                DataType = "double",
                DistributionConfig = new ExponentialDistributionConfig { Rate = 0.5 },
                Seed = _options.IsDeterministic ? random.Next(int.MinValue, int.MaxValue) : 0
            },
            new DataFrameColumn
            {
                Name = "Gamma",
                DataType = "double",
                DistributionConfig = new GammaDistributionConfig { Shape = 3, Scale = 3 },
                Seed = _options.IsDeterministic ? random.Next(int.MinValue, int.MaxValue) : 0
            },
            new DataFrameColumn
            {
                Name = "LogNormal",
                DataType = "double",
                DistributionConfig = new LogNormalDistributionConfig { Mean = 2, StandardDeviation = 3 },
                Seed = _options.IsDeterministic ? random.Next(int.MinValue, int.MaxValue) : 0
            },
            new DataFrameColumn
            {
                Name = "Uniform",
                DataType = "double",
                DistributionConfig = new UniformDistributionConfig { Min = 5, Max = 15 },
                Seed = _options.IsDeterministic ? random.Next(int.MinValue, int.MaxValue) : 0
            }
        };

        var command = new ShapiroWilkTestCommand(AnsiConsole.Console, null);
        //command.Test();

        //_apiKeyOptions.Update(x => x.OpenAI = "Test");

        //var appSettingsPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "DataCrafter", "appsettings.json");
        //string jsonData = File.ReadAllText(appSettingsPath);
        //var jsonNode = JsonNode.Parse(jsonData);
        //jsonNode["TestData"]["Name"] = "John";

        //var command = new NormalDistributionDetailsCommand(AnsiConsole.Console);
        //await command.Test();

        //var path = Path.Combine(Directory.GetCurrentDirectory(), "data.csv");

        //await _dataFrameCsvSink.StreamToCsv(columns, 1000, path, new ProgressTask(1, "Test", 1000, false));

        //var logNormalColumn = new DataFrameColumn
        //{
        //    Name = "LogNormal",
        //    Type = "double",
        //    DistributionConfig = new LogNormalDistributionConfig { Mean = 50, StandardDeviation = 5 },
        //    Seed = _options.IsDeterministic ? random.Next(int.MinValue, int.MaxValue) : 0
        //};

        //_distributionPlotterConsoleWriter.PlotPdf(logNormalColumn.Distribution as IUnivariateDistribution);



        //_dataFrameColumnConsoleWriter.PrintFieldsToConsole(fields);

        //var faker = _fakeProvider.BuildDynamicFaker(fields);
        //var data = faker.Generate(25);

        //_generatedDataConsoleSink.PrintDynamicClassToConsole(data);

        //var distributions = _distributionProvider.GetUnivariateDistributions();
        //double[] fittingData = data.Select(x => (double)(x.Attributes as IDictionary<string, object>).First().Value).ToArray();

        //var da = new DistributionAnalysis();
        //var univariateDistributions = _distributionProvider.GetUnivariateDistributions();
        //_distributionFittingConsoleWriter.WriteDistributionMetaDataToConsole(univariateDistributions, DistributionInfoType.Fittable);

        //foreach (var distribution in univariateDistributions)
        //{
        //    if (distribution is IFittableDistribution<double> fittableDistribution)
        //    {
        //        if (da.Distributions.All(x => x.ToString() != fittableDistribution.ToString()))
        //            da.Distributions.Add(fittableDistribution);
        //    }
        //}


        ////AnsiConsole.Write(distributionTable);

        //var fit = da.Learn(fittingData);

        //AnsiConsole.WriteLine();
        //AnsiConsole.WriteLine("Results");

        //var resultTable = new Table();
        //resultTable.AddColumn("Name");
        //resultTable.AddColumn("ChiSquare");
        //resultTable.AddColumn("ChiSquareRank");
        //resultTable.AddColumn("AndersonDarling");
        //resultTable.AddColumn("AndersonDarlingRank");
        //resultTable.AddColumn("KolmogorovSmirnov");
        //resultTable.AddColumn("KolmogorovSmirnovRank");

        //foreach (var distribution in fit)
        //{
        //    resultTable.AddRow(
        //        distribution.Name,
        //        distribution.ChiSquare.ToString("N2"),
        //        distribution.ChiSquareRank.ToString(),
        //        distribution.AndersonDarling.ToString("N2"),
        //        distribution.AndersonDarlingRank.ToString(),
        //        distribution.KolmogorovSmirnov.ToString("N2"),
        //        distribution.KolmogorovSmirnovRank.ToString()
        //    );
        //}

        //AnsiConsole.Write(resultTable);

        // -------------------------------------------------------

        //var da2 = new DistributionAnalysis();

        //AnsiConsole.WriteLine();
        //AnsiConsole.WriteLine("Available Distributions from Activator");
        //foreach (var distribution in _distributionProvider.Temp())
        //{
        //    if (distribution is IFittableDistribution<double> fittableDistribution && da2.Distributions.All(x => x.ToString() != fittableDistribution.ToString()))
        //    {
        //        da2.Distributions.Add(fittableDistribution);
        //        AnsiConsole.MarkupLine("[green]Included {0}[/]", fittableDistribution.ToString());
        //    }
        //    else
        //    {
        //        if (DistributionAnalysis.GetUnivariateDistributions().Any(x => x.Name == distribution.GetType().Name))
        //            AnsiConsole.MarkupLine("[darkorange3_1]Excluded {0}[/]", distribution.ToString());
        //        else
        //            AnsiConsole.MarkupLine("[red]Incompatible {0}[/]", distribution.ToString());
        //    }
        //}
        //var fit2 = da2.Learn(fittingData);

        //AnsiConsole.WriteLine();
        //AnsiConsole.WriteLine("Results");

        //var resultTable2 = new Table();
        //resultTable2.AddColumn("Name");
        //resultTable2.AddColumn("Index");
        //resultTable2.AddColumn("ChiSquare");
        //resultTable2.AddColumn("ChiSquareRank");
        //resultTable2.AddColumn("AndersonDarling");
        //resultTable2.AddColumn("AndersonDarlingRank");
        //resultTable2.AddColumn("KolmogorovSmirnov");
        //resultTable2.AddColumn("KolmogorovSmirnovRank");

        //foreach (var distribution in fit2)
        //{
        //    resultTable2.AddRow(
        //        distribution.Name,
        //        distribution.Index.ToString(),
        //        distribution.ChiSquare.ToString("N2"),
        //        distribution.ChiSquareRank.ToString(),
        //        distribution.AndersonDarling.ToString("N2"),
        //        distribution.AndersonDarlingRank.ToString(),
        //        distribution.KolmogorovSmirnov.ToString("N2"),
        //        distribution.KolmogorovSmirnovRank.ToString()
        //    );
        //}

        //AnsiConsole.Write(resultTable2);

    }
}
