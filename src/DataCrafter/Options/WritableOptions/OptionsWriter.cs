using System.Text.Json;
using System.Text.Json.Nodes;

namespace DataCrafter.Options.WritableOptions;
public sealed class OptionsWriter : IOptionsWriter
{
    public void UpdateOptions(Action<JsonNode> callback)
    {
        var appSettingsPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "DataCrafter", "appsettings.json");
        string jsonString = File.Exists(appSettingsPath) ? File.ReadAllText(appSettingsPath) : "{}";

        var writerOptions = new JsonWriterOptions { Indented = true };
        var documentOptions = new JsonDocumentOptions { CommentHandling = JsonCommentHandling.Skip };

        JsonNode jsonNode = JsonNode.Parse(jsonString, null, documentOptions) ?? new JsonObject();

        // modify document
        callback(jsonNode);

        using FileStream fileStream = File.Create(appSettingsPath);
        using var writer = new Utf8JsonWriter(fileStream, options: writerOptions);
        jsonNode.WriteTo(writer);
    }
}
