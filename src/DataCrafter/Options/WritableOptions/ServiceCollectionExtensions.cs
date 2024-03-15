using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace DataCrafter.Options.WritableOptions;
static class ServicesCollectionExtensions
{
    public static IServiceCollection ConfigureWritable<T>(
        this IServiceCollection services,
        IConfigurationRoot configuration,
        string sectionName) where T : class, new()
    {
        services.Configure<T>(configuration.GetSection(sectionName));

        services.AddTransient<IWritableOptions<T>>(provider =>
        {
            var options = provider.GetService<IOptionsMonitor<T>>();
            IOptionsWriter writer = new OptionsWriter();
            return new WritableOptions<T>(sectionName, writer, options!);
        });

        return services;
    }
}
