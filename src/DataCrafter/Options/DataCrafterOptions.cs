using Microsoft.Extensions.Options;

namespace DataCrafter.Options;
public class DataCrafterOptions : IOptions<DataCrafterOptions>
{
    /// <summary>
    ///     Name of the section to find the options in appsettings
    /// </summary>
    public static readonly string SectionName = "DataCrafter:DataGeneration";

    /// <summary>
    ///     Determines whether the data generated is deterministic or not.
    /// </summary>
    public bool IsDeterministic { get; set; }

    /// <summary>
    ///     The seed used for generating the deterministic seeds for distributions.
    /// </summary>
    public int Seed { get; set; }

    DataCrafterOptions IOptions<DataCrafterOptions>.Value => this;
}
