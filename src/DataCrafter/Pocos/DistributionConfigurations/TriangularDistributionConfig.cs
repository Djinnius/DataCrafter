namespace DataCrafter.Pocos.DistributionConfigurations;
internal sealed class TriangularDistributionConfig : IDistributionConfig
{
    public string DistributionType => "Triangular";
    public double Minimum { get; set; }
    public double Maximum { get; set; }
    public double Mode { get; set; }
}
