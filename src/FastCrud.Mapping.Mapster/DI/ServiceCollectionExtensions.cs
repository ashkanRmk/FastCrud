using FastCrud.Abstractions.Abstractions;
using Mapster;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace FastCrud.Mapping.Mapster.DI;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection UseMapster(this IServiceCollection services, Action<TypeAdapterConfig>? configure = null)
    {
        var cfg = TypeAdapterConfig.GlobalSettings;
        cfg.Default.EnableNonPublicMembers(true);
        configure?.Invoke(cfg);
        
        services.TryAddSingleton(cfg);
        services.TryAddSingleton<IObjectMapper, MapsterObjectMapper>();

        return services;
    }
}