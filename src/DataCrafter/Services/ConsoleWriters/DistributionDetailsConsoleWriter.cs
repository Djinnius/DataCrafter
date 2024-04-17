using Accord.Statistics.Distributions;
using DataCrafter.Reflection.OrderBy;
using DataCrafter.Services.Distributions;
using Spectre.Console;

namespace DataCrafter.Services.ConsoleWriters;

internal sealed partial class DistributionDetailsConsoleWriter : IDistributionDetailsConsoleWriter
{
    private readonly IDistributionInfoService _distributionInfoService;
    private readonly IAnsiConsole _ansiConsole;
    private readonly Dictionary<DistributionInfoType, IOrderBy> _orderFunctions;

    public DistributionDetailsConsoleWriter(IDistributionInfoService distributionInfoService, IAnsiConsole ansiConsole)
    {
        _distributionInfoService = distributionInfoService;
        _ansiConsole = ansiConsole;
        _orderFunctions =
            new Dictionary<DistributionInfoType, IOrderBy>
            {
                { DistributionInfoType.Name, new OrderBy<ExtendedDistributionInfo, string>(x => x.Name) },
                { DistributionInfoType.VariateType, new OrderBy<ExtendedDistributionInfo, bool>(x => x.IsUnivariate) },
                { DistributionInfoType.DataType, new OrderBy<ExtendedDistributionInfo, bool>(x => x.IsContinuous) },
                { DistributionInfoType.Fittable, new OrderBy<ExtendedDistributionInfo, bool>(x => x.IsFittable) },
                { DistributionInfoType.Sampleable, new OrderBy<ExtendedDistributionInfo, bool>(x => x.IsFittable) }
            };
    }

    public void WriteDistributionMetaDataToConsole(
        IEnumerable<IDistribution> distributions,
        DistributionInfoType colourBy = DistributionInfoType.Name,
        DistributionInfoType orderBy = DistributionInfoType.Name,
        bool orderByDescending = false)
    {
        _ansiConsole.WriteLine();

        var distributionTable = new Table()
            .LeftAligned()
            .Title("Distribution Information")
            .AddColumn("Name")
            .AddColumn("Variate Type")
            .AddColumn("Data Type")
            .AddColumn("Sampleable")
            .AddColumn("Fittable")
            .AddColumn("Fitting Options");

        var sortedDistributions = orderByDescending
            ? distributions.Select(_distributionInfoService.GetDistributionProperties).OrderByDescending(_orderFunctions[orderBy])
            : distributions.Select(_distributionInfoService.GetDistributionProperties).OrderBy(_orderFunctions[orderBy]);

        foreach (var distributionInfo in sortedDistributions)
            AddRow(distributionTable, distributionInfo, GetRowColour(colourBy, distributionInfo));

        _ansiConsole.Write(distributionTable);
    }

    private Color GetRowColour(DistributionInfoType colourBy, ExtendedDistributionInfo distributionInfo)
    {
        return colourBy switch
        {
            DistributionInfoType.Name => Color.White,
            DistributionInfoType.VariateType => GetVariateColour(distributionInfo),
            DistributionInfoType.DataType => GetDataTypeColour(distributionInfo),
            DistributionInfoType.Fittable => GetFittableColour(distributionInfo),
            DistributionInfoType.Sampleable => GetSampleableColour(distributionInfo),
            _ => Color.White,
        };
    }

    private Color GetVariateColour(ExtendedDistributionInfo distributionInfo)
        => distributionInfo.IsUnivariate ? Color.Violet : Color.Blue;

    private Color GetDataTypeColour(ExtendedDistributionInfo distributionInfo)
        => distributionInfo.IsContinuous ? Color.Yellow : Color.Purple;

    private Color GetFittableColour(ExtendedDistributionInfo distributionInfo)
        => distributionInfo.IsFittable ? Color.Green : Color.DarkOrange3_1;

    private Color GetSampleableColour(ExtendedDistributionInfo distributionInfo)
        => distributionInfo.IsSampleable ? Color.Lime : Color.DarkOrange;

    private void AddRow(Table distributionTable, ExtendedDistributionInfo distributionInfo, Color color)
    {
        var optionsType = distributionInfo.GetFittingOptions()?.GetType();
        var optionsString = optionsType != null ? GetPropertiesString(optionsType!) : string.Empty;

        var name = new Markup($"[{color}]{distributionInfo.Name}[/]");
        var variateString = distributionInfo.IsUnivariate || distributionInfo.IsMultivariate ? new Markup($"[{color}]{(distributionInfo.IsUnivariate ? "Univariate" : "Multivariate")}[/]") : new Markup(string.Empty);
        var dataTypeString = distributionInfo.IsContinuous ? new Markup($"[{color}]Continuous[/]") : distributionInfo.IsDiscrete ? new Markup($"[{color}]Discrete[/]") : new Markup(string.Empty);
        var sampleableString = new Markup($"[{color}]{(distributionInfo.IsSampleable ? "Sampleable" : "Not Sampleable")}[/]");
        var fittableString = new Markup($"[{color}]{(distributionInfo.IsFittable ? "Fittable" : "Not Fittable")}[/]");
        var options = new Markup($"[{color}]{optionsString}[/]");

        distributionTable.AddRow(name, variateString, dataTypeString, sampleableString, fittableString, options);
    }

    private string GetPropertiesString(Type type)
    {
        var className = type.Name;
        var properties = type.GetProperties();

        return $"{className}: {string.Join(",", properties.Select(x => x.Name))}";
    }
}
