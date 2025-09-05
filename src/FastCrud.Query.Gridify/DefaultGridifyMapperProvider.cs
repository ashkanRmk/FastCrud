using Gridify;

namespace FastCrud.Query.Gridify;

internal sealed class DefaultGridifyMapperProvider : IGridifyMapperProvider
{
    public IGridifyMapper<T>? GetMapper<T>() => null;
}