using DataCrafter.Entities;
using Spectre.Console;
using Spectre.Console.Rendering;

namespace DataCrafter.Services.ConsoleWriters;
internal sealed class GeneratedDataConsoleWriter : IGeneratedDataConsoleWriter
{
    private readonly IAnsiConsole _ansiConsole;

    public GeneratedDataConsoleWriter(IAnsiConsole ansiConsole)
    {
        _ansiConsole = ansiConsole;
    }

    public void PrintDynamicClassToConsole(List<DynamicClass> dynamicClasses)
    {
        _ansiConsole.WriteLine();
        _ansiConsole.Write(new Rule($"[u]Columns[/]").RuleStyle("grey").Centered());
        _ansiConsole.WriteLine();
        PrintTable(dynamicClasses);
        _ansiConsole.WriteLine();
    }

    public void PrintDynamicClassSamplesToConsole(List<DynamicClass> dynamicClasses)
    {
        _ansiConsole.WriteLine();
        PrintSamples(dynamicClasses);
        _ansiConsole.WriteLine();
    }

    private void PrintSamples(List<DynamicClass> dynamicClasses)
    {
        var table = new Table().Title("Generated Data");

        table.AddColumn("Id");

        foreach (var key in GetDistinctKeys(dynamicClasses.First()))
            table.AddColumn(key);

        var subLists = SplitList(dynamicClasses);

        for (int i = 0; i < subLists.Count; i++)
        {
            AddRows(table, subLists[i]);

            if (i< subLists.Count - 1)
                table.AddRow(Enumerable.Range(0, table.Columns.Count()).Select(x => new Markup("[grey]...[/]")));
        }

        _ansiConsole.Write(table);
    }

    private List<List<DynamicClass>> SplitList(List<DynamicClass> inputList)
    {
        var result = new List<List<DynamicClass>>();
        var sublist = new List<DynamicClass>();

        long previousId = -1;

        foreach (var item in inputList)
        {
            if (previousId != -1 && item.Id != previousId + 1)
            {
                // Discontinuity found, add current sublist to result
                result.Add(sublist);
                sublist = new List<DynamicClass>();
            }

            sublist.Add(item);
            previousId = item.Id;
        }

        // Add the last sublist
        if (sublist.Any())
        {
            result.Add(sublist);
        }

        return result;
    }

    private void PrintTable(List<DynamicClass> dynamicClasses)
    {
        var table = new Table().Title("Generated Data");

        // Add columns to the table
        table.AddColumn("Row");

        foreach (var key in GetDistinctKeys(dynamicClasses.First()))
        {
            table.AddColumn(key);
        }

        // Add rows to the table
        if (dynamicClasses.Count > 20)
        {
            AddRows(table, dynamicClasses.Take(10));
            table.AddRow(Enumerable.Range(0, table.Columns.Count()).Select(x => new Markup("[grey]...[/]")));
            AddRows(table, dynamicClasses.TakeLast(10));
        }
        else
        {
            AddRows(table, dynamicClasses);
        }

        // Render the table to the console
        _ansiConsole.Write(table);
    }

    private static void AddRows(Table table, IEnumerable<DynamicClass> dynamicClasses)
    {
        foreach (var dynamicClass in dynamicClasses)
            AddRow(table, dynamicClass);
    }

    private static void AddRow(Table table, DynamicClass dynamicClass)
    {
        var columnValues = new List<IRenderable>
            {
                new Markup(dynamicClass.Id.ToString())
            };

        if (dynamicClass.Attributes is not IDictionary<string, object> attributes)
            return;

        foreach (var value in attributes.Select(x => x.Value))
        {
            switch (value)
            {
                case double doubleValue:
                    // Format double values with comma separators for thousands, three decimal places and right align.
                    columnValues.Add(new Markup(string.Format("{0,10:N3}", doubleValue)));
                    break;
                case int intValue:
                    // Format integer values with comma separators for thousands and right align
                    columnValues.Add(new Markup(string.Format("{0,10:N0}", intValue)));
                    break;
                case decimal decimalValue:
                    // Format decimal values with comma separators for thousands, three decimal places and right-align
                    columnValues.Add(new Markup(string.Format("{0,10:F3}", decimalValue)));
                    break;
                default:
                    // For non-matching values, use default formatting
                    columnValues.Add(new Markup(value?.ToString() ?? string.Empty));
                    break;
            }
        }

        table.AddRow(columnValues);
    }

    static IEnumerable<string> GetDistinctKeys(DynamicClass dynamicClass)
        => dynamicClass.Attributes.Keys.Distinct().ToList();
}
