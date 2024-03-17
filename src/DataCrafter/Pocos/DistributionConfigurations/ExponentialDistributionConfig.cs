namespace DataCrafter.Pocos.DistributionConfigurations;

public class ExponentialDistributionConfig : IDistributionConfig
{
    public string DistributionType => "Exponential";
    public double Rate { get; set; }
}
