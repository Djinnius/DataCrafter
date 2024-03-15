namespace DataCrafter.DistributionConfigurations;

public class GammaDistributionConfig : IDistributionConfig
{
    public string DistributionType => "Gamma";
    public double Shape { get; set; }
    public double Scale { get; set; }
}
