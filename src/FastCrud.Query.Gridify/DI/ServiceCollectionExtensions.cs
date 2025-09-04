using FastCrud.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace FastCrud.Query.Gridify.DI;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection UseGridifyQueryEngine(this IServiceCollection services)
    {
        services.AddSingleton<IQueryEngine, GridifyQueryEngine>();
        return services;
    }
}
