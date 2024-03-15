namespace DataCrafter.Entities;

public class DynamicClass
{
    public long Id { get; set; }
    public IDictionary<string, object> Attributes { get; set; } = null!;
}
