using Mapster;
using FastCrud.Abstractions.Abstractions;

namespace FastCrud.Mapping.Mapster
{
    public sealed class MapsterObjectMapper(TypeAdapterConfig config) : IObjectMapper
    {
        public TDest Map<TDest>(object source)
        {
            return source.Adapt<TDest>(config);
        }

        public void Map(object source, object destination)
        {
            source.Adapt(destination, config);
        }
    }
}
