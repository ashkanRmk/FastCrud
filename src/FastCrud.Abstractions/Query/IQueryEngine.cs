using FastCrud.Abstractions.Primitives;

namespace FastCrud.Abstractions.Query;

/// <summary>
/// Abstraction over a query engine. Implementations apply filtering, sorting and paging to LINQ queries.
/// </summary>
public interface IQueryEngine
{
    /// <summary>
    /// Applies the supplied <see cref="IQuerySpec"/> to the query and returns a paged result.
    /// </summary>
    /// <typeparam name="TAgg">Aggregate type.</typeparam>
    /// <param name="query">Queryable.</param>
    /// <param name="spec">Query specification describing paging, filtering and sorting.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task<PagedResult<TAgg>> ApplyQueryAsync<TAgg>(IQueryable<TAgg> query, IQuerySpec spec, CancellationToken cancellationToken);
}