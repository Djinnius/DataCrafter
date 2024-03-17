namespace DataCrafter.Pocos.DistributionConfigurations;
internal sealed class ChiSquareDistributionConfig : IDistributionConfig
{
    public string DistributionType => "Chi-Square";
    public int DegreesOfFreedom { get; set; }
}
