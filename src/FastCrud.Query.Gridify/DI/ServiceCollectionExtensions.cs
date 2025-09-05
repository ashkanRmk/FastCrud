using System.Reflection;
using FastCrud.Abstractions.Query;
using Microsoft.Extensions.DependencyInjection;

namespace FastCrud.Query.Gridify.DI
{
    public static class GridifyServiceCollectionExtensions
    {
        /// <summary>
        /// Registers the Gridify query engine and a scanning mapper provider.
        /// </summary>
        /// <param name="services">The service collection.</param>
        /// <param name="assemblies">
        /// Assemblies to scan for IGridifyMapperProfile implementations.
        /// If none are provided, the entry/ executing assembly is scanned.
        /// </param>
        public static IServiceCollection UseGridifyQueryEngine(
            this IServiceCollection services,
            params Assembly[] assemblies)
        {
            services.AddSingleton<IGridifyMapperProvider>(sp => new GridifyMapperProvider(assemblies));
            services.AddSingleton<IQueryEngine, GridifyQueryEngine>(); // Your engine using IGridifyMapperProvider
            return services;
        }
        
    }
}
