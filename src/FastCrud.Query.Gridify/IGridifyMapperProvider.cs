using Gridify;

namespace FastCrud.Query.Gridify;

public interface IGridifyMapperProvider
{
    IGridifyMapper<T>? GetMapper<T>();
}