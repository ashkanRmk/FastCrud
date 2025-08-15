using System.Reflection;
using Gridify;

namespace FastCrud.Infrastructure;

public static class ServiceCollectionGridifyExtensions
{
    public static IServiceCollection AddGridifyMappersFromAssembly(
        this IServiceCollection services, Assembly assembly)
    {
        var mapperBase = typeof(GridifyMapper<>);

        var mapperTypes = assembly
            .GetTypes()
            .Where(t => !t.IsAbstract && t.BaseType is { IsGenericType: true } &&
                        t.BaseType.GetGenericTypeDefinition() == mapperBase)
            .ToList();

        foreach (var impl in mapperTypes)
        {
            var entityType = impl.BaseType!.GetGenericArguments()[0];
            var serviceType = mapperBase.MakeGenericType(entityType);

            services.AddSingleton(serviceType, sp => ActivatorUtilities.CreateInstance(sp, impl));
        }

        return services;
    }
}