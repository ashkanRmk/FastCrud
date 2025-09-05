using FastCrud.Abstractions;
using Mapster;
using System;

namespace FastCrud.Mapping.Mapster
{
    /// <summary>
    /// Mapster-based implementation of <see cref="IObjectMapper"/>.
    /// </summary>
    public sealed class MapsterObjectMapper : IObjectMapper
    {
        private readonly TypeAdapterConfig _config;
        /// <summary>
        /// Initializes a new instance with the supplied configuration.
        /// </summary>
        /// <param name="config">Mapster configuration.</param>
        public MapsterObjectMapper(TypeAdapterConfig config)
        {
            _config = config;
        }
        /// <inheritdoc />
        public TDest Map<TDest>(object source)
        {
            return source.Adapt<TDest>(_config);
        }
        /// <inheritdoc />
        public void Map(object source, object destination)
        {
            source.Adapt(destination, _config);
        }
    }
}
