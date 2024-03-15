namespace DataCrafter.DistributionConfigurations;
internal sealed class WeibullDistributionConfig : IDistributionConfig
{
    public string DistributionType => "Weibull";
    public double Shape { get; set; }
    public double Scale { get; set; }
}
