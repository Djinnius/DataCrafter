using System.Text.Json;
using System.Text.Json.Nodes;
using Microsoft.Extensions.Options;

namespace DataCrafter.Options.WritableOptions;
internal sealed class WritableOptions<T> : IWritableOptions<T> where T : class, new()
{
    private readonly string _sectionName;
    private readonly IOptionsWriter _writer;
    private readonly IOptionsMonitor<T> _options;

    public WritableOptions(
        string sectionName,
        IOptionsWriter writer,
        IOptionsMonitor<T> options)
    {
        _sectionName = sectionName;
        _writer = writer;
        _options = options;
    }

    public T Value => _options.CurrentValue;

    public void Update(Action<T> applyChanges)
    {
        _writer.UpdateOptions(jsonNode =>
        {
            // Split the section name into segments
            string[] sectionNames = _sectionName.Split(':');

            // Start with the root object
            JsonObject? sectionObject = jsonNode.Root.AsObject();

            // Navigate to the correct section, creating it if necessary
            foreach (string sectionName in sectionNames)
            {
                if (!sectionObject!.ContainsKey(sectionName))
                {
                    sectionObject[sectionName] = new JsonObject();
                }

                sectionObject = sectionObject[sectionName] as JsonObject;
            }

            // Deserialize the section to the options object
            T optionsObject = JsonSerializer.Deserialize<T>(sectionObject!.ToString()) ?? new T();

            // Apply the changes to the options object
            applyChanges(optionsObject);

            string json = JsonSerializer.Serialize(optionsObject);
            JsonObject? updatedSectionObject = JsonSerializer.Deserialize<JsonObject>(json);

            // Merge the updated section object with the existing one
            foreach (var property in updatedSectionObject!)
            {
                sectionObject[property.Key] = property.Value!.DeepClone();
            }
        });
    }
}
