namespace FastCrud.Abstractions.Primitives;
public sealed record PagedResult<T>(
    IReadOnlyList<T> Items, 
    long TotalItems, 
    int Page, 
    int PageSize);
