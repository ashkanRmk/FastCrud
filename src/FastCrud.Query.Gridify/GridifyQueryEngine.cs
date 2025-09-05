using Gridify;
using FastCrud.Abstractions.Primitives;
using FastCrud.Abstractions.Query;
using Microsoft.EntityFrameworkCore;

namespace FastCrud.Query.Gridify;

public sealed class GridifyQueryEngine(IGridifyMapperProvider mappers) : IQueryEngine
{
    public async Task<PagedResult<TOut>> ApplyQueryAsync<TIn, TOut>(
        IQueryable<TIn> source, 
        IQuerySpec spec, 
        Func<IQueryable<TIn>, IQueryable<TOut>> projector, 
        CancellationToken cancellationToken = default)
    {
        var gq = new GridifyQuery
        {
            Filter   = spec.Filter,
            OrderBy  = spec.OrderBy,
            Page     = spec.Page  > 0 ? spec.Page  : 1,
            PageSize = spec.PageSize > 0 ? spec.PageSize : 50
        };

        var mapper = mappers.GetMapper<TIn>();

        if (!string.IsNullOrWhiteSpace(gq.Filter))
            source = mapper is null ? source.ApplyFiltering(gq) : source.ApplyFiltering(gq, mapper);

        if (!string.IsNullOrWhiteSpace(gq.OrderBy))
            source = mapper is null ? source.ApplyOrdering(gq) : source.ApplyOrdering(gq, mapper);

        var total = source.Count();

        if (spec.PageSize > 0)
            source = source.ApplyPaging(gq);

        var projected = projector(source);
        var items = await projected.ToListAsync(cancellationToken);

        var result = new PagedResult<TOut>(items, total, gq.Page, gq.PageSize);
        return result;
    }
}