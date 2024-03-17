namespace DataCrafter.Pocos.DistributionConfigurations;
internal sealed class LogisticDistributionConfig : IDistributionConfig
{
    public string DistributionType => "Logistic";
    public double Location { get; set; }
    public double Scale { get; set; }
}
