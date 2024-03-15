using DataCrafter.Options.WritableOptions;

namespace DataCrafter.Options;
internal sealed class DefaultOptionsService : IDefaultOptionsService
{
    private readonly IWritableOptions<DataCrafterOptions> _dataCrafterOptions;

    public DefaultOptionsService(IWritableOptions<DataCrafterOptions> dataCrafterOptions)
    {
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
        }
    }
}
