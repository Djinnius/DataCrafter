using DataCrafter.Options;
using DataCrafter.Options.WritableOptions;
using Spectre.Console;

namespace DataCrafter.Services.Options;
internal sealed class DefaultOptionsService : IDefaultOptionsService
{
    private readonly IAnsiConsole _ansiConsole;
    private readonly IWritableOptions<DataCrafterOptions> _dataCrafterOptions;

    public DefaultOptionsService(
        IAnsiConsole ansiConsole,
        IWritableOptions<DataCrafterOptions> dataCrafterOptions)
    {
        _ansiConsole = ansiConsole;
        _dataCrafterOptions = dataCrafterOptions;
    }

    public void SetDefaultValues()
    {
        var appDataPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "DataCrafter");
        var appSettingsFileName = "appsettings.json";
        var appSettingsPath = Path.Combine(appDataPath, appSettingsFileName);

        Directory.CreateDirectory(appDataPath);

        if (!File.Exists(appSettingsPath))
        {
            _dataCrafterOptions.Update(x =>
            {
                x.IsDeterministic = true;
                x.Seed = 999;
            });

            _ansiConsole.MarkupLine($"Default settings file created: [green]{appSettingsPath}[/]");
        }
    }
}
