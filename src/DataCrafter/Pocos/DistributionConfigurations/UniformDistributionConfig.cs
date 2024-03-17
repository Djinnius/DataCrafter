namespace DataCrafter.Pocos.DistributionConfigurations;

public class UniformDistributionConfig : IDistributionConfig
{
    public string DistributionType => "Uniform";
    public double Min { get; set; }
    public double Max { get; set; }
}
