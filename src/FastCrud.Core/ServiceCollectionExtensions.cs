using FastCrud.Abstractions;
using FastCrud.Core.Services;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace FastCrud.Core
{
    /// <summary>
    /// Service registration extensions for the FastCrud core services.
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Registers the core FastCrud services with the dependency injection container. This adds a scoped open-generic
        /// implementation of <see cref="ICrudService{TAgg, TId}"/>, a singleton UTC clock and a default query engine.
        /// Callers can optionally configure options via the supplied action.
        /// </summary>
        /// <param name="services">The service collection.</param>
        /// <param name="configure">Optional configuration for future options.</param>
        /// <returns>The service collection for chaining.</returns>
        public static IServiceCollection AddFastCrudCore(this IServiceCollection services, Action<FastCrudOptions>? configure = null)
        {
            services.AddScoped(typeof(ICrudService<,>), typeof(CrudService<,>));
            services.AddSingleton<IFastCrudClock, FastCrudClock>();
            // register default simple query engine; can be overridden via UseGridifyQueryEngine
            services.AddSingleton<IQueryEngine, SimpleQueryEngine>();
            configure?.Invoke(new FastCrudOptions());
            return services;
        }
    }

    /// <summary>
    /// Options for FastCrud. Reserved for future extensibility.
    /// </summary>
    public class FastCrudOptions
    {
        // currently empty; reserved for future settings (e.g., soft-delete, auditing)
    }

    internal class FastCrudClock : IFastCrudClock
    {
        public DateTime UtcNow => DateTime.UtcNow;
    }
}