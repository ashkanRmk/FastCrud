using FastCrud.Abstractions.Primitives;

namespace FastCrud.Abstractions.Query;

public interface IQueryEngine
{
    Task<PagedResult<TOut>> ApplyQueryAsync<TIn, TOut>(
        IQueryable<TIn> source,
        IQuerySpec spec,
        Func<IQueryable<TIn>, IQueryable<TOut>> projector,
        CancellationToken cancellationToken = default);
}