using System.Globalization;
using Bogus;
using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.TypeConversion;
using DataCrafter.Entities;
using DataCrafter.Services.Bogus;
using Spectre.Console;

namespace DataCrafter.Services.FileIO;

///<inheritdoc cref="IDataFrameCsvSink"/>
internal sealed class DataFrameCsvSink : IDataFrameCsvSink
{
    private readonly IAnsiConsole _ansiConsole;
    private readonly IFakeProvider _fakeProvider;

    public DataFrameCsvSink(
        IAnsiConsole ansiConsole,
        IFakeProvider fakeProvider)
    {
        _ansiConsole = ansiConsole;
        _fakeProvider = fakeProvider;
    }

    public async Task<List<DynamicClass>> StreamToCsv(IList<IDataFrameColumn> dataFrameColumns, int numberOfRows, string outputPath, ProgressTask progressTask)
    {
        try
        {
            var faker = _fakeProvider.BuildDynamicFaker(dataFrameColumns);

            using var writer = new StreamWriter(outputPath);
            using var csvWriter = new CsvWriter(writer, new CsvConfiguration(CultureInfo.InvariantCulture));
            ConfigureDateTimeFormatting(csvWriter);

            await WriteCsvHeaders(dataFrameColumns, csvWriter);
            var samples = await WriteCsvBody(dataFrameColumns, faker, numberOfRows, csvWriter, progressTask);
            return samples;
        }
        catch (Exception ex)
        {
            _ansiConsole.WriteException(ex, ExceptionFormats.ShortenEverything);
            return new List<DynamicClass>();
        }
    }

    private static void ConfigureDateTimeFormatting(CsvWriter csvWriter)
    {
        var dateTimeOptions = new TypeConverterOptions { Formats = new[] { "yyyy-MM-dd hh:mm:ss" } };
        var dateOptions = new TypeConverterOptions { Formats = new[] { "yyyy-MM-dd" } };
        csvWriter.Context.TypeConverterOptionsCache.AddOptions<DateTime>(dateTimeOptions);
        csvWriter.Context.TypeConverterOptionsCache.AddOptions<DateTime?>(dateTimeOptions);
        csvWriter.Context.TypeConverterOptionsCache.AddOptions<DateOnly>(dateOptions);
        csvWriter.Context.TypeConverterOptionsCache.AddOptions<DateOnly?>(dateOptions);
    }

    private async Task<List<DynamicClass>> WriteCsvBody(
        IEnumerable<IDataFrameColumn> dataFrameColumns,
        Faker<DynamicClass> faker,
        int numberOfRows,
        CsvWriter csv,
        ProgressTask progressTask)
    {
        var sampleRowNumbers = GetSamplingRowNumbers(numberOfRows, 10, 2);
        var samples = new List<DynamicClass>();
        progressTask.MaxValue = numberOfRows;
        progressTask.StartTask();

        for (int i = 0; i < numberOfRows; i++)
        {
            var record = faker.Generate();

            if (sampleRowNumbers.Contains(i))
                samples.Add(record);

            foreach (var column in dataFrameColumns)
                csv.WriteField(record.Attributes[column.Name]);

            await csv.NextRecordAsync();
            progressTask.Increment(1);
        }

        progressTask.StopTask();
        return samples;
    }

    private static async Task WriteCsvHeaders(IEnumerable<IDataFrameColumn> dataFrameColumns, CsvWriter csv)
    {
        foreach (var column in dataFrameColumns)
            csv.WriteField(column.Name);

        await csv.NextRecordAsync();
    }

    public List<int> GetSamplingRowNumbers(int rowCount, int numberOfSamplesPerBucket, int numberOfBuckets)
    {
        if (rowCount < (numberOfSamplesPerBucket * numberOfBuckets))
            return Enumerable.Range(0, rowCount).ToList();

        // Calculate the number of rows to skip between each bucket
        int skipRows = (rowCount - numberOfSamplesPerBucket * numberOfBuckets) / (numberOfBuckets - 1);
        var sampledRows = new List<int>();
        var currentRow = 0;

        for (int i = 0; i < numberOfBuckets; i++)
        {
            sampledRows.AddRange(Enumerable.Range(currentRow, currentRow + numberOfSamplesPerBucket));
            currentRow = currentRow + numberOfSamplesPerBucket + skipRows;
        }

        return sampledRows;
    }
}
