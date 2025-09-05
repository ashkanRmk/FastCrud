using Gridify;
using FastCrud.Abstractions.Primitives;
using FastCrud.Abstractions.Query;

namespace FastCrud.Query.Gridify;

public sealed class GridifyQueryEngine(IGridifyMapperProvider mappers) : IQueryEngine
{
    public Task<PagedResult<T>> ApplyQueryAsync<T>(IQueryable<T> source, IQuerySpec spec, CancellationToken ct = default)
    {
        var gq = new GridifyQuery
        {
            Filter = spec.Filter,
            OrderBy = spec.OrderBy,
            Page = spec.Page > 0 ? spec.Page : 1,
            PageSize = spec.PageSize > 0 ? spec.PageSize : 50
        };
        
        var mapper = mappers.GetMapper<T>();

        if (!string.IsNullOrWhiteSpace(gq.Filter))
            source = mapper is null ? source.ApplyFiltering(gq) : source.ApplyFiltering(gq, mapper);

        if (!string.IsNullOrWhiteSpace(gq.OrderBy))
            source = mapper is null ? source.ApplyOrdering(gq) : source.ApplyOrdering(gq, mapper);

        var total = source.Count();

        if (spec.PageSize > 0)
            source = source.ApplyPaging(gq);

        var items = source.ToList();

        var result = new PagedResult<T>(items, total, gq.Page, gq.PageSize);
        return Task.FromResult(result);
    }


}