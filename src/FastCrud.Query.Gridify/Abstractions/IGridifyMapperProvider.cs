using Gridify;

namespace FastCrud.Query.Gridify.Abstractions;

public interface IGridifyMapperProvider
{
    IGridifyMapper<T>? GetMapper<T>();
}