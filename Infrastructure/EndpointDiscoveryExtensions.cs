using System.Reflection;

namespace FastCrud.Infrastructure;

public static class EndpointDiscoveryExtensions
{
    public static void MapEndpointsFromAssembly(
        this IEndpointRouteBuilder app, Assembly assembly, string versionPrefix = "/v1")
    {
        var group = app.MapGroup(versionPrefix);
        var sp = app.ServiceProvider;

        var modules = assembly.GetTypes()
            .Where(t => !t.IsAbstract && typeof(IEndpointModule).IsAssignableFrom(t))
            .Select(t => (IEndpointModule)ActivatorUtilities.CreateInstance(sp, t))
            .ToList();

        foreach (var m in modules)
            m.Register(group);
    }
}