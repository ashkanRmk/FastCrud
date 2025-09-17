using System.Reflection;
using FastCrud.Abstractions.Query;
using FastCrud.Query.Gridify.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace FastCrud.Query.Gridify.DI;

public static class GridifyServiceCollectionExtensions
{
    public static IServiceCollection UseGridifyQueryEngine(
        this IServiceCollection services,
        params Assembly[] assemblies)
    {
        if (assemblies is { Length: > 0 })
        {
            services.AddSingleton<IGridifyMapperProvider>(sp => new GridifyMapperProvider(assemblies));
        }
        else
        {
            var entry = Assembly.GetEntryAssembly();
            var calling = Assembly.GetCallingAssembly();
            var executing = Assembly.GetExecutingAssembly();
            var uniq = new[] { entry, calling, executing }
                .Where(a => a != null)
                .Distinct()
                .ToArray()!;
            services.AddSingleton<IGridifyMapperProvider>(sp => new GridifyMapperProvider(uniq!));
        }

        services.AddSingleton<IQueryEngine, GridifyQueryEngine>();
        return services;
    }
        
}