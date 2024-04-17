using DataCrafter.Commands.DataFrame.CsvStatistics;
using DataCrafter.Commands.DataFrame.DetectDistributionOutliers;
using DataCrafter.Commands.DataFrame.FitDistributionsToCsvColumn;
using DataCrafter.Commands.DataFrame.Generate;
using DataCrafter.Commands.DataFrame.PlotCsvColumn;
using DataCrafter.Commands.DataFrame.ShapiroWilk;
using DataCrafter.Commands.DataFrame.SummariseCsv;
using DataCrafter.Commands.DataFrameColumns.Cauchy;
using DataCrafter.Commands.DataFrameColumns.Clear;
using DataCrafter.Commands.DataFrameColumns.Exponential;
using DataCrafter.Commands.DataFrameColumns.Gamma;
using DataCrafter.Commands.DataFrameColumns.Load;
using DataCrafter.Commands.DataFrameColumns.LogNormal;
using DataCrafter.Commands.DataFrameColumns.Normal;
using DataCrafter.Commands.DataFrameColumns.Plot;
using DataCrafter.Commands.DataFrameColumns.Print;
using DataCrafter.Commands.DataFrameColumns.Save;
using DataCrafter.Commands.DataFrameColumns.Uniform;
using DataCrafter.Commands.Distributions.Details;
using DataCrafter.Commands.Distributions.DistributionTypesSummary;
using DataCrafter.Commands.GPT;
using DataCrafter.Commands.StatisticalDefinitions;
using DataCrafter.DependencyInjection;
using DataCrafter.Options;
using DataCrafter.Options.WritableOptions;
using DataCrafter.Services.Bogus;
using DataCrafter.Services.ConsoleWriters;
using DataCrafter.Services.DataTypeServices;
using DataCrafter.Services.Distributions;
using DataCrafter.Services.FileIO;
using DataCrafter.Services.Main;
using DataCrafter.Services.Mappers;
using DataCrafter.Services.Options;
using DataCrafter.Services.Repositories;
using DataCrafter.Services.Spectre;
using FluentValidation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Spectre.Console;
using Spectre.Console.Cli;

static IConfigurationRoot LoadConfiguration()
{
    var appDataPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "DataCrafter");
    var appSettingsFileName = "appsettings.json";
    var appSettingsPath = Path.Combine(appDataPath, appSettingsFileName);

    Directory.CreateDirectory(appDataPath);

    return new ConfigurationBuilder()
        .SetBasePath(appDataPath)
        .AddJsonFile(appSettingsPath, optional: true, reloadOnChange: true)
        .Build();
}

var configuration = LoadConfiguration();

// Note: New tree resolved for every command in the command line i.e. Singleton instances
// are not shared between commands.
var serviceCollection = new ServiceCollection()
            .AddScoped<Main>()
            .AddSingleton(AnsiConsole.Console) // Can inject IAnsiConsole
            .AddSingleton<IGeneratedDataConsoleWriter, GeneratedDataConsoleWriter>()
            .AddSingleton<IDistributionPlotterConsoleWriter, DistributionPlotterConsoleWriter>()
            .AddSingleton<IDataFrameColumnConsoleWriter, DataFrameColumnConsoleWriter>()
            .AddSingleton<IDistributionColorMapper, DistributionColorMapper>()
            .AddSingleton<IDistributionProvider, DistributionProvider>()
            .AddSingleton<IDistributionInfoService, DistributionInfoService>()
            .AddSingleton<IDistributionDetailsConsoleWriter, DistributionDetailsConsoleWriter>()
            .AddSingleton<ITableSchemaRepository, TableSchemaRepository>()
            .AddSingleton<IDataTypeProvider, DataTypeProvider>()
            .AddSingleton<IFakeProvider, FakeProvider>()
            .AddSingleton<IDataFrameCsvSink, DataFrameCsvSink>()
            .AddSingleton<IDefaultOptionsService, DefaultOptionsService>()
            .AddSingleton<IOptionsWriter, OptionsWriter>()
            .AddSingleton<ITableProvider, TableProvider>()
            .AddSingleton(typeof(IWritableOptions<>), typeof(WritableOptions<>))
            .AddTransient<EmptyCommandSettings>()
            .AddTransient<PrintDataFrameColumnsCommandSettings>()
            .AddTransient<SaveDataFrameColumnsCommandSettings>()
            .AddTransient<LoadDataFrameColumnsCommandSettings>()
            .AddTransient<AddCauchyDataFrameColumnCommandSettings>()
            .AddTransient<IValidator<AddCauchyDataFrameColumnCommandSettings>, AddCauchyDataFrameColumnCommandSettingsValidator>()
            .AddTransient<AddNormalDataFrameColumnCommandSettings>()
            .AddTransient<IValidator<AddNormalDataFrameColumnCommandSettings>, AddNormalDataFrameColumnCommandSettingsValidator>()
            .AddTransient<AddExponentialDataFrameColumnCommandSettings>()
            .AddTransient<IValidator<AddExponentialDataFrameColumnCommandSettings>, AddExponentialDataFrameColumnCommandSettingsValidator>()
            .AddTransient<AddGammaDataFrameColumnCommandSettings>()
            .AddTransient<IValidator<AddGammaDataFrameColumnCommandSettings>, AddGammaDataFrameColumnCommandSettingsValidator>()
            .AddTransient<AddLogNormalDataFrameColumnCommandSettings>()
            .AddTransient<IValidator<AddLogNormalDataFrameColumnCommandSettings>, AddLogNormalDataFrameColumnCommandSettingsValidator>()
            .AddTransient<AddUniformDataFrameColumnCommandSettings>()
            .AddTransient<IValidator<AddUniformDataFrameColumnCommandSettings>, AddUniformDataFrameColumnCommandSettingsValidator>()
            .AddTransient<GenerateDataCommandSettings>()
            .AddTransient<IValidator<GenerateDataCommandSettings>, GenerateDataCommandSettingsValidator>()
            .AddTransient<PlotDistributionCommandSettings>()
            .AddTransient<IValidator<PlotDistributionCommandSettings>, PlotDistributionCommandSettingsValidator>()
            .AddTransient<PrintDistributionTypeSummaryTableCommandSettings>()
            .AddTransient<IValidator<PrintDistributionTypeSummaryTableCommandSettings>, PrintDistributionTypeSummaryTableCommandSettingsValidator>()
            .AddTransient<PlotCsvColumnCommandSettings>()
            .AddTransient<IValidator<PlotCsvColumnCommandSettings>, PlotCsvColumnCommandSettingsValidator>()
            .AddTransient<FitDistributionsToCsvColumnCommandSettings>()
            .AddTransient<IValidator<FitDistributionsToCsvColumnCommandSettings>, FitDistributionsToCsvColumnCommandSettingsValidator>()
            .AddTransient<ShapiroWilkTestCommandSettings>()
            .AddTransient<IValidator<ShapiroWilkTestCommandSettings>, ShapiroWilkTestCommandSettingsValidator>()
            .AddTransient<DetectDistributionOutliersCommandSettings>()
            .AddTransient<IValidator<DetectDistributionOutliersCommandSettings>, DetectDistributionOutliersCommandSettingsValidator>()
            .AddTransient<SummariseCsvCommandSettings>()
            .AddTransient<CsvStatisticsCommandSettings>()
            .ConfigureWritable<DataCrafterOptions>(configuration, DataCrafterOptions.SectionName)
            .ConfigureWritable<ApiKeysOptions>(configuration, ApiKeysOptions.SectionName);

var registrar = new TypeRegistrar(serviceCollection);
var app = new CommandApp(registrar);

app.Configure(config =>
{
    config.SetApplicationName("datacrafter");

    config.AddBranch("columns", columns =>
    {
        columns.SetDescription("View, clear, and add or remove columns.");

        columns.AddCommand<PrintDataFrameColumnsCommand>("print")
            .WithAlias("p")
            .WithDescription("Prints the current loaded column configuration.")
            .WithExample(["columns", "print"])
            .WithExample(["columns", "print", "--columnsAsRows", "true"]);

        columns.AddCommand<ClearDataFrameColumnsCommand>("clear")
            .WithAlias("c")
            .WithDescription("Clears the current loaded column configuration.")
            .WithExample(["columns", "clear"]);

        columns.AddCommand<SaveDataFrameColumnsCommand>("save")
            .WithAlias("s")
            .WithDescription("Saves the current loaded column configuration.")
            .WithExample(["columns", "save"]);

        columns.AddCommand<LoadDataFrameColumnsCommand>("load")
            .WithAlias("l")
            .WithDescription("Loads a column configuration.")
            .WithExample(["columns", "load"])
            .WithExample(["columns", "load", "-i", "columns.json"]);

        columns.AddCommand<PlotDistributionCommand>("plot")
            .WithAlias("pl")
            .WithDescription("Plots the distribution for the provided column.")
            .WithExample(["columns", "plot", "Normal_Column"]);

        columns.AddBranch("add", add =>
        {
            add.SetDescription("Adds a distribution");

            add.AddCommand<AddCauchyDataFrameColumnCommand>("cauchy")
                .WithAlias("c")
                .WithDescription("Adds a column to the configuration with distribution of type Cauchy.")
                .WithExample(["columns", "add", "cauchy", "MyColumn", "int", "5", "1", "-o", "5", "-s", "555"]);

            add.AddCommand<AddNormalDataFrameColumnCommand>("normal")
                .WithAlias("n")
                .WithDescription("Adds a column to the configuration with distribution of type Normal.")
                .WithExample(["columns", "add", "normal", "MyColumn", "int", "5", "1", "-o", "5", "-s", "555"]);

            add.AddCommand<AddExponentialDataFrameColumnCommand>("exponential")
                .WithAlias("e")
                .WithDescription("Adds a column to the configuration with distribution of type Exponential.")
                .WithExample(["columns", "add", "exponential", "MyColumn", "int", "0.5", "-o", "5", "-s", "555"]);

            add.AddCommand<AddGammaDataFrameColumnCommand>("gamma")
                .WithAlias("g")
                .WithDescription("Adds a column to the configuration with distribution of type Gamma.")
                .WithExample(["columns", "add", "gamma", "MyColumn", "int", "0.5", "2", "-o", "5", "-s", "555"]);

            add.AddCommand<AddLogNormalDataFrameColumnCommand>("lognormal")
                .WithAlias("ln")
                .WithDescription("Adds a column to the configuration with distribution of type LogNormal.")
                .WithExample(["columns", "add", "lognormal", "MyColumn", "int", "5", "1", "-o", "5", "-s", "555"]);

            add.AddCommand<AddUniformDataFrameColumnCommand>("uniform")
                .WithAlias("u")
                .WithDescription("Adds a column to the configuration with distribution of type Uniform.")
                .WithExample(["columns", "add", "uniform", "MyColumn", "int", "1", "100", "-o", "5", "-s", "555"]);

        }).WithAlias("a");
    }).WithAlias("c");

    config.AddBranch("data", data =>
    {
        data.SetDescription("Generate or plot data.");

        data.AddCommand<GenerateDataCommand>("generate")
        .WithAlias("g")
        .WithDescription("Saves generated data to CSV file.")
        .WithExample(["data", "generate", "1000"]);

        data.AddCommand<SummariseCsvCommand>("summarisecsv")
        .WithAlias("s")
        .WithDescription("Summarises the contents of a csv file.")
        .WithExample(["data", "summarisecsv"]);

        data.AddCommand<CsvStatisticsCommand>("csvstatistics")
        .WithAlias("c")
        .WithDescription("Prints the csv column statistics.")
        .WithExample(["data", "csvstatistics"]);

        data.AddCommand<PlotCsvColumnCommand>("plot")
        .WithAlias("p")
        .WithDescription("Plots the provided column name as a historgram.")
        .WithExample(["data", "plot", "ColumnName"]);

        data.AddCommand<FitDistributionsToCsvColumnCommand>("fit")
        .WithAlias("f")
        .WithDescription("Ranks distributions for the given csv column data.")
        .WithExample(["data", "fit", "ColumnName"]);

        data.AddCommand<ShapiroWilkTestCommand>("shapirowilk")
        .WithAlias("shap")
        .WithDescription("Performs the shapiro wilk test on the provided column to test for a Normal distribution.")
        .WithExample(["data", "shapirowilk", "ColumnName"]);

        data.AddCommand<DetectDistributionOutliersCommand>("outliers")
        .WithAlias("o")
        .WithDescription("Fits a specified distribution to column data and determines outliers under a given p value threshold for the distribution's probability density function.")
        .WithExample(["data", "outliers", "ColumnName", "Normal"]);
    }
    ).WithAlias("d");

    config.AddBranch("distributions", distributions =>
    {
        distributions.SetDescription("Details on available distributions.");

        distributions.AddBranch("summary", summary =>
        {
            summary.SetDescription("Summaries on distribution types.");

            summary.AddCommand<PrintDistributionTypeSummaryTableCommand>("table")
            .WithAlias("t")
            .WithDescription("Prints a summary of distribution details.")
            .WithExample(["distributions", "summary", "table"]);

            summary.AddCommand<NormalDistributionDetailsCommand>("normal")
            .WithAlias("n")
            .WithDescription("Gets summary details for normal distributions.")
            .WithExample(["distributions", "summary", "normal"]);

        }).WithAlias("s");
    }).WithAlias("di");

    config.AddBranch("definitions", definitions =>
    {
        definitions.SetDescription("Statistical definitions and examples.");

        definitions.AddCommand<SkewnessKurtosisDetailsCommand>("skewness")
           .WithAlias("sk")
           .WithDescription("Prints a description of Skewness and kurtosis.")
           .WithExample(["definitions", "skewness"]);

        definitions.AddCommand<SkewnessKurtosisDetailsCommand>("kurtosis")
           .WithAlias("ku")
           .WithDescription("Prints a description of Skewness and kurtosis.")
           .WithExample(["definitions", "kurtosis"]);

    }).WithAlias("def");

    config.AddBranch("gpt", gpt =>
    {
        gpt.SetDescription("Access to chat GPT.");

        gpt.AddCommand<GetGptModelsCommand>("getmodels")
           .WithAlias("gm")
           .WithDescription("Gets a list of currently available models.")
           .WithExample(["gpt", "getmodels"]);

    }).WithAlias("g");

    //#if DEBUG
    config.PropagateExceptions();
    config.ValidateExamples();
    //#endif
});


#if DEBUG
    await RunDebug(serviceCollection);
#else
    await Run(app, serviceCollection, args);
#endif

// Code to run the tool when installed as a tool.
#pragma warning disable CS8321 // Local function is declared but never used
static async Task Run(CommandApp app, IServiceCollection serviceCollection, string[] args)
{
    try
    {
        var serviceProvider = serviceCollection.BuildServiceProvider();
        OnRun(serviceProvider);
        await app.RunAsync(args);
    }
    catch (Exception e)
    {
        AnsiConsole.WriteException(e);
    }
}

// Code to run when debugging, in this case whatever test code is in Main.
// To test the tool, requires installing, see the install.bat file.
static async Task RunDebug(IServiceCollection serviceCollection)
{
    var serviceProvider = serviceCollection.BuildServiceProvider();
    OnRun(serviceProvider);
    var main = serviceProvider.GetRequiredService<Main>();
    await main.Execute();
}
#pragma warning restore CS8321 // Local function is declared but never used

// Common code when running both in debug and as a release tool.
static void OnRun(ServiceProvider serviceProvider)
{
    var defaultOptionsService = serviceProvider.GetRequiredService<IDefaultOptionsService>();
    defaultOptionsService.SetDefaultValues();
}
