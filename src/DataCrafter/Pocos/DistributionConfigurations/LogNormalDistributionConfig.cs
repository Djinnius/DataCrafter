namespace DataCrafter.Pocos.DistributionConfigurations;

public class LogNormalDistributionConfig : IDistributionConfig
{
    public string DistributionType => "Log Normal";
    public double Mean { get; set; }
    public double StandardDeviation { get; set; }
}
