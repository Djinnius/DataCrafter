using Accord.Statistics.Distributions;
using Accord.Statistics.Distributions.Univariate;

namespace DataCrafter.Services.Distributions;
internal sealed class DistributionProvider : IDistributionProvider
{
    public IList<IUnivariateDistribution> GetFittableUnivariateDistributions()
    {
        return GetUnivariateDistributions().Where(x => x is IFittable<double>).ToList();
    }

    //public IList<IUnivariateDistribution> Temp()
    //{
    //    var distributions = new List<IUnivariateDistribution>();

    //    foreach (var distributionType in DistributionAnalysis.GetUnivariateDistributions())
    //    {
    //        var defaultConstructor = distributionType.GetConstructor(Type.EmptyTypes);

    //        if (typeof(IUnivariateDistribution).IsAssignableFrom(distributionType) && !distributionType.IsAbstract && defaultConstructor != null)
    //        {
    //            var instance = (IUnivariateDistribution)Activator.CreateInstance(distributionType);
    //            distributions.Add(instance);
    //        }
    //    }

    //    return distributions.OrderBy(x => x.ToString()).ToList();
    //}

    public IList<IUnivariateDistribution> GetUnivariateDistributions()
    {
        return new List<IUnivariateDistribution>
        {
            //new AndersonDarlingDistribution(AndersonDarlingDistributionType.Uniform, 50), // requires samples
            //new AndersonDarlingDistribution(AndersonDarlingDistributionType.Normal, 50), // requires samples
            new BernoulliDistribution(),
            new BetaDistribution(),
            //new BetaPrimeDistribution(0.5, 0.5), // default parameters, need justification
            new BinomialDistribution(),
            new BirnbaumSaundersDistribution(),
            new CauchyDistribution(),
            new ChiSquareDistribution(),
            new DegenerateDistribution(),
            //new EmpiricalDistribution(), // requires samples
            new EmpiricalHazardDistribution(),
            new ExponentialDistribution(),
            new FDistribution(),
            new FoldedNormalDistribution(),
            new GammaDistribution(),
            //new GeneralContinuousDistribution(), // needs parameters
            new GeneralDiscreteDistribution(),
            //new GeneralizedBetaDistribution(), // alpha & beta?
            //new GeneralizedNormalDistribution(), // location, scale, shape?
            new GeneralizedParetoDistribution(),
            //new GeometricDistribution(), // probability of success
            //new GompertzDistribution(), // eta and b
            //new GrubbDistribution(), // number of samples
            new GumbelDistribution(),
            new HyperbolicSecantDistribution(),
            //new HypergeometricDistribution(), // population size, successes and samples?
            //new InverseChiSquareDistribution(), // degrees of freedom,
            //new InverseGammaDistribution(), // shape and scale
            //new InverseGaussianDistribution(), // mean and shape
            //new KolmogorovSmirnovDistribution(), // samples
            //new KumaraswamyDistribution(), // a and b
            //new LaplaceDistribution(),
            new LevyDistribution(),
            new LogisticDistribution(),
            new LogLogisticDistribution(),
            new LognormalDistribution(),
            //new MannWhitneyDistribution().Fit, // n1, n2
            //new NakagamiDistribution(), // shape and spread
            //new NegativeBinomialDistribution(), // failures, probability
            //new NoncentralTDistribution(),
            new NormalDistribution(),
            new ParetoDistribution(),
            new PoissonDistribution(),
            //new PowerLognormalDistribution(), // power and shape
            //new PowerNormalDistribution(),
            new RademacherDistribution(),
            //new RayleighDistribution(), // sigma
            //new ShapiroWilkDistribution(), // samples
            //new RayleighDistribution() // Sigma
            //new ShapiroWilkDistribution(), // samples
            new ShiftedLogLogisticDistribution(),
            new SkewNormalDistribution(),
            //new SymmetricGeometricDistribution(), // probability of success
            //new TDistribution(), // degrees of freedom
            //new TrapezoidalDistribution(), // a, b, c, d
            //new TriangularDistribution(), // min, max, mode
            //new TukeyLambdaDistribution(), // lambda
            new UniformContinuousDistribution(),
            //new UniformDiscreteDistribution(), // a, b
            //new UQuadraticDistribution(), // a, b
            new VonMisesDistribution(),
            //new WeibullDistribution(), // shape, scale
            //new WilcoxonDistribution(), // n
            //new WrappedCauchyDistribution(), // mu, gamma
        }.OrderBy(x => x.ToString()).ToList();
    }
}
