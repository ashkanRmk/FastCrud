using FastCrud.Abstractions;
using Mapster;

namespace FastCrud.Mapping.Mapster;

public sealed class MapsterObjectMapper : IObjectMapper
{
    private readonly TypeAdapterConfig _config;
    public MapsterObjectMapper(TypeAdapterConfig config) => _config = config;
    public TDest Map<TDest>(object source) => source.Adapt<TDest>(_config);
}
