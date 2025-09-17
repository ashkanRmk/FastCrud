using System.Collections.Concurrent;
using Mapster;
using FastCrud.Abstractions.Abstractions;

namespace FastCrud.Mapping.Mapster;

public sealed class MapsterObjectMapper(TypeAdapterConfig config) : IObjectMapper
{
    private static readonly ConcurrentDictionary<(Type Src, Type Dst), Delegate> Cache = new();

    public TDest Map<TDest>(object source)
        => source.Adapt<TDest>(config);

    public void Map(object source, object destination)
    {
        ArgumentNullException.ThrowIfNull(source);
        ArgumentNullException.ThrowIfNull(destination);

        var key = (Src: source.GetType(), Dst: destination.GetType());

        var del = Cache.GetOrAdd(key, static (k, cfg) =>
        {
            var mi = typeof(TypeAdapterConfig)
                .GetMethod(nameof(TypeAdapterConfig.GetMapToTargetFunction))!
                .MakeGenericMethod(k.Src, k.Dst);

            return (Delegate)mi.Invoke(cfg, null)!;
        }, config);

        del.DynamicInvoke(source, destination);
    }
}