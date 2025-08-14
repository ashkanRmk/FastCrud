namespace Crud.Generator.Repositories;

public sealed class QueryResult<T>
{
    public required IReadOnlyList<T> Items { get; init; }
    public required int TotalItems { get; init; }
    public int Page { get; init; }
    public int PageSize { get; init; }
}