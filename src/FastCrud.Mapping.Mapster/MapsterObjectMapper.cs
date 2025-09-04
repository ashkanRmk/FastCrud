using FastCrud.Abstractions;
using Mapster;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace FastCrud.Mapping.Mapster
{
    /// <summary>
    /// Mapster-based implementation of <see cref="IObjectMapper"/>.
    /// </summary>
    public sealed class MapsterObjectMapper : IObjectMapper
    {
        private readonly TypeAdapterConfig _config;
        /// <summary>
        /// Initializes a new instance with the supplied configuration.
        /// </summary>
        /// <param name="config">Mapster configuration.</param>
        public MapsterObjectMapper(TypeAdapterConfig config)
        {
            _config = config;
        }
        /// <inheritdoc />
        public TDest Map<TDest>(object source)
        {
            return source.Adapt<TDest>(_config);
        }
        /// <inheritdoc />
        public void Map(object source, object destination)
        {
            source.Adapt(destination, _config);
        }
    }

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
}
