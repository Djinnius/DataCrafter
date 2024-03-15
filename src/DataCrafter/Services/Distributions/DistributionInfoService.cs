using System.Reflection;
using Accord.Statistics.Distributions;
using Accord.Statistics.Distributions.Reflection;

namespace DataCrafter.Services.Distributions;
internal sealed class DistributionInfoService : IDistributionInfoService
{
    public ExtendedDistributionInfo GetDistributionProperties(IDistribution distribution)
        => new ExtendedDistributionInfo(distribution.GetType());
}

internal sealed class ExtendedDistributionInfo : DistributionInfo
{
    public ExtendedDistributionInfo(Type type) : base(type)
    {
    }

    public bool IsFittable
    {
        get
        {
            TypeInfo typeInfo = DistributionType.GetTypeInfo();
            return typeof(IFittable<double>).GetTypeInfo().IsAssignableFrom(typeInfo)
                || typeof(IFittable<double[]>).GetTypeInfo().IsAssignableFrom(typeInfo);
        }
    }

    public bool IsSampleable
    {
        get
        {
            TypeInfo typeInfo = DistributionType.GetTypeInfo();
            return typeof(ISampleableDistribution<double>).GetTypeInfo().IsAssignableFrom(typeInfo)
                || typeof(ISampleableDistribution<double[]>).GetTypeInfo().IsAssignableFrom(typeInfo);
        }
    }

    public string Description
    {
        get
        {
            switch (DistributionType.Name)
            {
                case "Beta":
                    return "The Beta distribution is a continuous probability distribution defined on the interval [0, 1].";

                case "Bernoulli":
                    return "A discrete probability distribution of a random variable that takes binary values (0 or 1).";

                case "Binomial":
                    return "Represents the number of successes in a fixed number of independent Bernoulli trials.";

                case "BirnbaumSaunders":
                    return "A continuous probability distribution used in reliability engineering to model time until failure.";

                case "GeneralDiscrete":
                    return "A generalization of the discrete distribution without specifying particular properties.";

                case "Cauchy":
                    return "A symmetric distribution with heavy tails, and undefined mean and variance.";

                case "Degenerate":
                    return "A distribution concentrated at a single point (a degenerate distribution has no variability).";

                case "Exponential":
                    return "Models the time between events in a Poisson process.";

                case "F":
                    return "Used in statistical hypothesis testing, representing the ratio of two independent chi-squared distributions.";

                case "FoldedNormal":
                    return "A folded version of the normal distribution, where negative values are reflected to positive.";

                case "Gumbel":
                    return "Used in extreme value theory, often to model the distribution of maximum values.";

                case "EmpiricalHazard":
                    return "Represents the cumulative hazard function for a sample of survival times.";

                case "Levy":
                    return "Heavy-tailed distribution used in finance for modeling asset returns.";

                case "ShiftedLogLogistic":
                    return "A location-scale family of distributions with log-logistic shape.";

                case "Logistic":
                    return "S-shaped continuous probability distribution.";

                case "LogLogistic":
                    return "A distribution often used in survival analysis, representing time until a certain event.";

                case "Lognormal":
                    return "Distribution of a random variable whose logarithm is normally distributed.";

                case "Normal":
                    return "Bell-shaped symmetric distribution frequently used in statistical analysis.";

                case "Pareto":
                    return "Represents a power-law distribution where a small number of items account for the majority of the values.";

                case "GeneralizedPareto":
                    return "A family of distributions that includes the Pareto distribution as a special case.";

                case "Poisson":
                    return "Models the number of events occurring within a fixed interval of time or space.";

                case "Rademacher":
                    return "Discrete distribution with values +1 and -1 with equal probability.";

                case "HyperbolicSecant":
                    return "Continuous distribution with heavy tails, used in signal processing and communication theory.";

                case "SkewNormal":
                    return "Generalization of the normal distribution to include skewness.";

                case "UniformContinuous":
                    return "All values within a specified range are equally likely.";

                case "VonMises":
                    return "Circular distribution used in directional statistics.";

                case "Gamma":
                    return "Generalization of the exponential distribution, often used in reliability and queuing theory.";

                case "ChiSquare":
                    return "Distribution of the sum of squared standard normal deviates.";

                default:
                    return "Description not available.";
            }
        }
    }

    public string Examples
    {
        get
        {
            switch (DistributionType.Name)
            {
                case "Beta":
                    return "Used in Bayesian statistics to model the distribution of probabilities. Example: Probability of success in a series of trials.";

                case "Bernoulli":
                    return "Modeling the outcome of a coin toss (heads or tails). Example: Probability of heads in a single coin toss.";

                case "Binomial":
                    return "Modeling the number of successful shots in a basketball game with repeated attempts. Example: Number of successful coin tosses in a series.";

                case "BirnbaumSaunders":
                    return "Predicting the time until a machine fails. Example: Time until a component fails in a reliability test.";

                case "GeneralDiscrete":
                    return "Customized distribution for specific discrete data. Example: Modeling the distribution of custom events.";

                case "Cauchy":
                    return "Used in physics to model resonance behavior. Example: Modeling the behavior of a resonant system.";

                case "Degenerate":
                    return "Modeling a constant value with no variability. Example: Representing a fixed point in a distribution.";

                case "Exponential":
                    return "Modeling the time between arrivals of customers at a service point. Example: Time between phone calls at a call center.";

                case "F":
                    return "Analysis of variance (ANOVA). Example: Comparing means of multiple groups in an experimental study.";

                case "FoldedNormal":
                    return "Modeling the absolute values of normally distributed errors. Example: Modeling the positive deviations from a mean value.";

                case "Gumbel":
                    return "Used in extreme value theory, often to model the distribution of maximum values. Example: Predicting extreme temperatures.";

                case "EmpiricalHazard":
                    return "Analyzing time-to-event data in survival analysis. Example: Estimating hazard function from observed survival times.";

                case "Levy":
                    return "Modeling financial market returns. Example: Analyzing the distribution of daily stock price changes.";

                case "ShiftedLogLogistic":
                    return "Reliability analysis in engineering. Example: Predicting the time until failure of a mechanical component.";

                case "Logistic":
                    return "Modeling growth curves in biology. Example: Predicting the population growth of a species.";

                case "LogLogistic":
                    return "Analyzing survival times in medical studies. Example: Predicting the time until recurrence of a disease.";

                case "Lognormal":
                    return "Modeling stock prices. Example: Modeling the distribution of closing prices in financial markets.";

                case "Normal":
                    return "Heights of a population. Example: Modeling the distribution of heights in a human population.";

                case "Pareto":
                    return "Wealth distribution in a population. Example: Modeling the distribution of income in a society.";

                case "GeneralizedPareto":
                    return "Extreme value analysis in environmental science. Example: Modeling extreme weather events.";

                case "Poisson":
                    return "Count of customer arrivals at a service point. Example: Modeling the number of phone calls received in an hour.";

                case "Rademacher":
                    return "Used in random walks and certain types of cryptography. Example: Modeling coin flips in cryptography algorithms.";

                case "HyperbolicSecant":
                    return "Modeling signal noise in communication systems. Example: Analyzing the noise level in a communication channel.";

                case "SkewNormal":
                    return "Modeling asymmetric data. Example: Analyzing the distribution of income with skewness.";

                case "UniformContinuous":
                    return "Modeling random variables with equal probability in a given range. Example: Randomly selecting a number between two values.";

                case "VonMises":
                    return "Modeling wind directions. Example: Analyzing the distribution of wind directions at a specific location.";

                case "Gamma":
                    return "Modeling waiting times in a queue. Example: Analyzing the time between arrivals in a service system.";

                case "ChiSquare":
                    return "Statistical hypothesis testing, such as testing variance. Example: Testing the variance of a sample.";

                default:
                    return "Examples not available.";
            }
        }
    }
}


