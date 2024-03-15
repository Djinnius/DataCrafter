namespace DataCrafter.DistributionConfigurations;

public class CauchyDistributionConfig : IDistributionConfig
{
    public string DistributionType => "Cauchy";
    public double Location { get; set; }
    public double Scale { get; set; }
}
