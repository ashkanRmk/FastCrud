using Gridify;

namespace FastCrud.Query.Gridify.Abstractions;

public interface IGridifyMapperProfile<T>
{
    void Configure(GridifyMapper<T> mapper);
}