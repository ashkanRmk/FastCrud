using FastCrud.Abstractions.Query;
using FastCrud.Core;
using Microsoft.Extensions.DependencyInjection;

namespace FastCrud.Query.Gridify.DI
{
    /// <summary>
    /// Extensions for registering the Gridify-based query engine. At present this simply aliases the simple query engine.
    /// A full Gridify implementation can be plugged in here in the future.
    /// </summary>
    public static class GridifyServiceCollectionExtensions
    {
        /// <summary>
        /// Overrides the default query engine with the Gridify query engine. Currently this registers the simple query engine.
        /// </summary>
        /// <param name="services">The service collection.</param>
        /// <returns>The service collection for chaining.</returns>
        public static IServiceCollection UseGridifyQueryEngine(this IServiceCollection services)
        {
            // Replace existing IQueryEngine registration with SimpleQueryEngine. In a full implementation this would register a GridifyQueryEngine.
            services.AddSingleton<IQueryEngine, SimpleQueryEngine>();
            return services;
        }
    }
}
