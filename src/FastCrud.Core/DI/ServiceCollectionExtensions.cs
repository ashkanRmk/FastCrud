using FastCrud.Abstractions.Abstractions;
using FastCrud.Abstractions.Query;
using FastCrud.Core.Services;
using Microsoft.Extensions.DependencyInjection;

namespace FastCrud.Core.DI
{
    /// <summary>
    /// Service registration extensions for the FastCrud core services.
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Registers the core FastCrud services with the dependency injection container. This adds a scoped open-generic
        /// implementation of <see cref="ICrudService{TAgg, TId}"/>.
        /// Callers can optionally configure options via the supplied action.
        /// </summary>
        /// <param name="services">The service collection.</param>
        /// <param name="configure">Optional configuration for future options.</param>
        /// <returns>The service collection for chaining.</returns>
        public static IServiceCollection AddFastCrudCore(this IServiceCollection services, Action<FastCrudOptions>? configure = null)
        {
            services.AddScoped(typeof(ICrudService<,>), typeof(CrudService<,>));
            
            configure?.Invoke(new FastCrudOptions());
            return services;
        }
    }
}