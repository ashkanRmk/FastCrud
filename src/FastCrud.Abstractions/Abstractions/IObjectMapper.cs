namespace FastCrud.Abstractions.Abstractions;

/// <summary>
/// Abstraction over an object mapper. Implementations map between DTOs and domain entities.
/// </summary>
public interface IObjectMapper
{
    /// <summary>
    /// Maps an arbitrary source object to a destination type.
    /// </summary>
    /// <typeparam name="TDest">Destination type.</typeparam>
    /// <param name="source">Source object.</param>
    /// <returns>The mapped object.</returns>
    TDest Map<TDest>(object source);

    /// <summary>
    /// Maps an arbitrary source object onto an existing destination instance.
    /// </summary>
    /// <param name="source">Source object.</param>
    /// <param name="destination">Destination object to mutate.</param>
    void Map(object source, object destination);
}