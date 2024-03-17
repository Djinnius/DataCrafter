namespace DataCrafter.Pocos.DistributionConfigurations;

public class NormalDistributionConfig : IDistributionConfig
{
    public string DistributionType => "Normal";
    public double Mean { get; set; }
    public double StandardDeviation { get; set; }
}
