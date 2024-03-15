using Microsoft.Extensions.Options;

namespace DataCrafter.Options;
public class ApiKeysOptions : IOptions<ApiKeysOptions>
{
    /// <summary>
    ///     Name of the section to find the options in appsettings
    /// </summary>
    public static readonly string SectionName = "DataCrafter:ApiKeys";

    /// <summary>
    ///     Gets or sets the Open AI key.
    /// </summary>
    public string OpenAI { get; set; } = string.Empty;

    ApiKeysOptions IOptions<ApiKeysOptions>.Value => this;
}
