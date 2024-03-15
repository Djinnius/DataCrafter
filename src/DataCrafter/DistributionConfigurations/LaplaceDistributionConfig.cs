namespace DataCrafter.DistributionConfigurations;
internal sealed class LaplaceDistributionConfig : IDistributionConfig
{
    public string DistributionType => "Laplace";
    public double Location { get; set; }
    public double Scale { get; set; }
}
