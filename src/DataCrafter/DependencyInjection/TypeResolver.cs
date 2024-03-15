using Microsoft.Extensions.DependencyInjection;
using Spectre.Console.Cli;

namespace DataCrafter.DependencyInjection;

internal sealed class TypeResolver : ITypeResolver
{
    private readonly IServiceProvider _provider;

    public TypeResolver(IServiceProvider provider)
    {
        _provider = provider ?? throw new ArgumentNullException(nameof(provider));
    }

    public object? Resolve(Type? type)
    {
        return type == null ? throw new ArgumentNullException(nameof(type)) : _provider.GetRequiredService(type);
    }
}
