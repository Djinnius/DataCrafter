using DataCrafter.Entities;
using DataCrafter.Services.ConsoleWriters;
using DataCrafter.Services.FileIO;
using DataCrafter.Services.Repositories;
using FluentValidation;
using Spectre.Console;
using Spectre.Console.Cli;

namespace DataCrafter.Commands.DataFrame.Generate;

internal sealed class GenerateDataCommand : AsyncCommand<GenerateDataCommandSettings>
{
    private readonly ITableSchemaRepository _tableSchemaRepository;
    private readonly IDataFrameCsvSink _dataFrameCsvSink;
    private readonly IGeneratedDataConsoleWriter _generatedDataConsoleWriter;
    private readonly IValidator<GenerateDataCommandSettings> _validator;
    private readonly IAnsiConsole _ansiConsole;

    public GenerateDataCommand(
        ITableSchemaRepository tableSchemaRepository,
        IDataFrameCsvSink dataFrameCsvSink,
        IGeneratedDataConsoleWriter generatedDataConsoleWriter,
        IValidator<GenerateDataCommandSettings> validator,
        IAnsiConsole ansiConsole)
    {
        _tableSchemaRepository = tableSchemaRepository;
        _dataFrameCsvSink = dataFrameCsvSink;
        _generatedDataConsoleWriter = generatedDataConsoleWriter;
        _validator = validator;
        _ansiConsole = ansiConsole;
    }

    public override async Task<int> ExecuteAsync(CommandContext context, GenerateDataCommandSettings settings)
    {
        if (ValidateSettings(settings) < 0)
            return -1;

        var dataFrameColumns = _tableSchemaRepository.GetAllDataFrameColumns().ToList();

        if (!dataFrameColumns.Any())
        {
            _ansiConsole.MarkupLine("[red]Error: No columns in configuration[/]");
            return -1;
        }

        var outputFileName = GetOutputFileName(settings.Output);
        var outputPath = GetOutputPath(settings.Output);
        var fullOutputPath = Path.Combine(outputPath, outputFileName);
        var samples = new List<DynamicClass>();

        await _ansiConsole.Progress()
            .AutoClear(false)   // Do not remove the task list when done
            .Columns(
            [
                new TaskDescriptionColumn(),    // Task description
                new ProgressBarColumn(),        // Progress bar
                new PercentageColumn(),         // Percentage
                new RemainingTimeColumn(),      // Remaining time
                new SpinnerColumn(),            // Spinner
            ])
            .StartAsync(async ctx =>
            {
                var progressTask = ctx.AddTask("Saving to CSV", new ProgressTaskSettings
                {
                    AutoStart = false
                });

                samples = await ExportCSV(settings, dataFrameColumns, fullOutputPath, progressTask);
            });

        if (samples.Any())
            _generatedDataConsoleWriter.PrintDynamicClassSamplesToConsole(samples);

        return 0;
    }

    private async Task<List<DynamicClass>> ExportCSV(GenerateDataCommandSettings settings, List<IDataFrameColumn> dataFrameColumns, string fullOutputPath, ProgressTask progressTask)
    {
        try
        {
            var samples = await _dataFrameCsvSink.StreamToCsv(dataFrameColumns, settings.Count, fullOutputPath, progressTask);
            _ansiConsole.MarkupLine($"[green]Columns successfully saved to: {fullOutputPath}[/]");

            return samples;
        }
        catch (Exception ex)
        {
            _ansiConsole.MarkupLine($"[red]Error saving columns.[/]");
            _ansiConsole.WriteException(ex, ExceptionFormats.ShortenEverything);
            return new List<DynamicClass>();
        }
    }

    private int ValidateSettings(GenerateDataCommandSettings settings)
    {
        var validationResult = _validator.Validate(settings);

        if (!validationResult.IsValid)
        {
            foreach (var error in validationResult.Errors)
                _ansiConsole.MarkupLine($"[red]Validation Error: {error.ErrorMessage}[/]");

            return -1;
        }

        return 0;
    }

    private string GetOutputFileName(FlagValue<string> output)
    {
        if (output.IsSet)
        {
            var fileName = Path.GetFileNameWithoutExtension(output.Value) ?? output.Value;
            return $"{fileName}.csv";
        }

        return "data.csv";
    }

    private string GetOutputPath(FlagValue<string> output)
    {
        if (output.IsSet)
        {
            var directory = Path.GetDirectoryName(output.Value);
            return string.IsNullOrEmpty(directory) ? Directory.GetCurrentDirectory() : directory;
        }

        return Directory.GetCurrentDirectory();
    }
}
