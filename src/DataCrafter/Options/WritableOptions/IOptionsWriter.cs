using System.Text.Json.Nodes;

namespace DataCrafter.Options.WritableOptions;

public interface IOptionsWriter
{
    void UpdateOptions(Action<JsonNode> callback);
}
