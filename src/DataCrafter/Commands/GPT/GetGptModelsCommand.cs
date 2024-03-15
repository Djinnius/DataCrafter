using System.Text.Json;
using DataCrafter.Options;
using DataCrafter.Options.WritableOptions;
using Spectre.Console;
using Spectre.Console.Cli;

namespace DataCrafter.Commands.GPT;
internal sealed class GetGptModelsCommand : AsyncCommand
{
    private readonly IWritableOptions<ApiKeysOptions> _apiKeysOptions;
    private readonly IAnsiConsole _ansiConsole;

    public GetGptModelsCommand(IWritableOptions<ApiKeysOptions> apiKeyOptions, IAnsiConsole ansiConsole)
    {
        _apiKeysOptions = apiKeyOptions;
        _ansiConsole = ansiConsole;
    }

    public override async Task<int> ExecuteAsync(CommandContext context)
    {
        var openaiApiKey = GetOpenAiKey();

        using var httpClient = new HttpClient();
        string apiUrl = "https://api.openai.com/v1/models";
        httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {openaiApiKey}");

        try
        {
            // Make the GET request
            HttpResponseMessage response = await httpClient.GetAsync(apiUrl);

            if (response.IsSuccessStatusCode)
            {
                string responseBody = await response.Content.ReadAsStringAsync();
                JsonDocument jsonDocument = JsonDocument.Parse(responseBody);
                var modelIds = ExtractModelIdsFromResponse(jsonDocument);
                PrintModelIdsToConsole(modelIds);
                return 0;
            }
            else
            {
                _ansiConsole.MarkupLine($"[red]Error: {response.StatusCode} - {response.ReasonPhrase}[/]");
                return -1;
            }
        }
        catch (Exception ex)
        {
            _ansiConsole.WriteException(ex, ExceptionFormats.ShortenPaths);
            return -1;
        }
    }

    private static List<string> ExtractModelIdsFromResponse(JsonDocument jsonDocument)
    {
        List<string> modelIds = new List<string>();
        JsonElement dataArray = jsonDocument.RootElement.GetProperty("data");
        foreach (JsonElement model in dataArray.EnumerateArray())
        {
            string id = model.GetProperty("id").GetString() ?? string.Empty;

            if (!string.IsNullOrEmpty(id))
                modelIds.Add(id);
        }

        return modelIds;
    }

    private void PrintModelIdsToConsole(List<string> modelIds)
    {
        var table = new Table().Border(TableBorder.Rounded);
        table.AddColumn("Model IDs");

        foreach (var modelId in modelIds.OrderBy(x => x))
            table.AddRow(modelId);

        _ansiConsole.Write(table);
    }

    private string GetOpenAiKey()
    {
        var openaiApiKey = _apiKeysOptions.Value.OpenAI;

        if (string.IsNullOrWhiteSpace(openaiApiKey))
        {
            openaiApiKey = _ansiConsole.Prompt(new TextPrompt<string>("Enter your OpenAI API Key:") { IsSecret = true });
            _apiKeysOptions.Update(x => x.OpenAI = openaiApiKey);
        }

        return openaiApiKey;
    }
}
