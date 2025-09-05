using FastCrud.Abstractions.Primitives;

namespace FastCrud.Abstractions.Query;

/// <summary>
/// Abstraction over a query engine. Implementations apply filtering, sorting and paging to LINQ queries.
/// </summary>
public interface IQueryEngine
{
    Task<PagedResult<TOut>> ApplyQueryAsync<TIn, TOut>(
        IQueryable<TIn> source,
        IQuerySpec spec,
        Func<IQueryable<TIn>, IQueryable<TOut>> projector,
        CancellationToken cancellationToken = default);
}