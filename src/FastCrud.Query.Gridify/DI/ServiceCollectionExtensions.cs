using System.Reflection;
using FastCrud.Abstractions.Query;
using FastCrud.Query.Gridify.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace FastCrud.Query.Gridify.DI
{
    public static class GridifyServiceCollectionExtensions
    {
        public static IServiceCollection UseGridifyQueryEngine(
            this IServiceCollection services,
            params Assembly[] assemblies)
        {
            services.AddSingleton<IGridifyMapperProvider>(sp => new GridifyMapperProvider(assemblies));
            services.AddSingleton<IQueryEngine, GridifyQueryEngine>();
            return services;
        }
        
    }
}
