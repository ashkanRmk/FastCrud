using FastCrud.Abstractions;
using Mapster;
using Microsoft.Extensions.DependencyInjection;

namespace FastCrud.Mapping.Mapster.DI;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection UseMapster(this IServiceCollection services, Action<TypeAdapterConfig>? configure = null)
    {
        var cfg = TypeAdapterConfig.GlobalSettings.Clone();
        configure?.Invoke(cfg);
        services.AddSingleton(cfg);
        services.AddSingleton<IObjectMapper, MapsterObjectMapper>();
        return services;
    }
}
