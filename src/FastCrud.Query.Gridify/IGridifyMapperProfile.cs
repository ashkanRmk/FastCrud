using Gridify;

namespace FastCrud.Query.Gridify;

/// <summary>
/// Implement this interface to configure a Gridify mapper for a specific entity type.
/// </summary>
/// <typeparam name="T">The entity type.</typeparam>
public interface IGridifyMapperProfile<T>
{
    void Configure(GridifyMapper<T> mapper);
}