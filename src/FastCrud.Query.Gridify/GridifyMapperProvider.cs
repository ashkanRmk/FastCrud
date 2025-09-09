using System.Collections.Concurrent;
using System.Reflection;
using FastCrud.Query.Gridify.Abstractions;
using Gridify;

namespace FastCrud.Query.Gridify;

public sealed class GridifyMapperProvider : IGridifyMapperProvider
{
    private readonly ConcurrentDictionary<Type, object> _cache = new();
    private readonly Assembly[] _assemblies;
    
    public GridifyMapperProvider(params Assembly[] assemblies)
    {
        _assemblies = (assemblies is { Length: > 0 })
            ? assemblies
            : [Assembly.GetExecutingAssembly()];

        BuildCache();
    }

    public IGridifyMapper<T>? GetMapper<T>()
    {
        return _cache.TryGetValue(typeof(T), out var mapper)
            ? (GridifyMapper<T>)mapper
            : null;
    }

    private void BuildCache()
    {
        var openProfile = typeof(IGridifyMapperProfile<>);

        var profiles = _assemblies
            .SelectMany(SafeDefinedTypes)
            .Where(t => t is { IsAbstract: false, IsInterface: false })
            .SelectMany(t => t.GetInterfaces()
                .Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == openProfile)
                .Select(i => new { ProfileType = t.AsType(), EntityType = i.GetGenericArguments()[0] }))
            .GroupBy(x => x.EntityType);

        foreach (var group in profiles)
        {
            var entityType = group.Key;

            var mapper = CreateMapperFor(entityType);

            foreach (var p in group.Select(g => g.ProfileType))
            {
                var profile = Activator.CreateInstance(p)!;
                var profileInterface = typeof(IGridifyMapperProfile<>).MakeGenericType(entityType);
                var configure = profileInterface.GetMethod(nameof(IGridifyMapperProfile<object>.Configure))!;
                configure.Invoke(profile, [mapper]);
            }

            _cache[entityType] = mapper;
        }
    }

    private static IEnumerable<TypeInfo> SafeDefinedTypes(Assembly a)
    {
        try { return a.DefinedTypes; }
        catch (ReflectionTypeLoadException ex) { return ex.Types.Where(t => t != null)!.Select(t => t!.GetTypeInfo()); }
    }
    
    private static object CreateMapperFor(Type entityType)
    {
        var mi = typeof(GridifyMapperProvider)
            .GetMethod(nameof(CreateMapperGeneric), BindingFlags.NonPublic | BindingFlags.Static)!;
        return mi.MakeGenericMethod(entityType).Invoke(null, null)!;
    }

    private static GridifyMapper<T> CreateMapperGeneric<T>() => new();
}
