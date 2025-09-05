using FastCrud.Abstractions;
using FastCrud.Abstractions.Abstractions;
using Mapster;
using Microsoft.Extensions.DependencyInjection;

namespace FastCrud.Mapping.Mapster;

/// <summary>
/// Extension methods for registering Mapster as the object mapper for FastCrud.
/// </summary>
public static class MapsterServiceCollectionExtensions
{
    /// <summary>
    /// Registers Mapster with the DI container and configures the global settings. Optionally accepts a configuration action
    /// to customize mapping rules.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configure">Optional configuration action.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection UseMapster(this IServiceCollection services, Action<TypeAdapterConfig>? configure = null)
    {
        var cfg = TypeAdapterConfig.GlobalSettings.Clone();
        configure?.Invoke(cfg);
        services.AddSingleton(cfg);
        services.AddSingleton<IObjectMapper, MapsterObjectMapper>();
        return services;
    }
}