using System.Text.Json.Serialization;

namespace DataCrafter.DistributionConfigurations;

[JsonDerivedType(typeof(UniformDistributionConfig), "Uniform")]
[JsonDerivedType(typeof(LogNormalDistributionConfig), "LogNormal")]
[JsonDerivedType(typeof(GammaDistributionConfig), "Gamma")]
[JsonDerivedType(typeof(ExponentialDistributionConfig), "Exponential")]
[JsonDerivedType(typeof(NormalDistributionConfig), "Normal")]
[JsonDerivedType(typeof(CauchyDistributionConfig), "Cauchy")]
public interface IDistributionConfig
{
    string DistributionType { get; }
}
