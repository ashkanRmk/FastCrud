using FastCrud.Abstractions;
using FastCrud.Abstractions.Primitives;
using FastCrud.Abstractions.Query;
using Gridify;

namespace FastCrud.Query.Gridify;

public class GridifyQueryEngine : IQueryEngine
{
    public Task<PagedResult<T>> ApplyAsync<T>(IQueryable<T> query, IQuerySpec spec, CancellationToken ct = default)
    {
        var gq = new GridifyQuery();

        if (spec.Filters?.Count > 0)
        {
            // Basic AND-join of filters: field op value (op defaults to ==)
            var parts = spec.Filters.Select(f => $"{f.Field}{(string.IsNullOrWhiteSpace(f.Op) ? "==" : f.Op)}{f.Value}");
            gq.Filter = string.Join(" && ", parts);
        }
        if (spec.Sorts?.Count > 0)
        {
            gq.OrderBy = string.Join(",", spec.Sorts.Select(s => $"{s.Field}{(s.Descending ? " desc" : "")}"));
        }
        var page = spec.Page ?? 1;
        var size = spec.PageSize ?? 20;

        var q = query.ApplyFiltering(gq);
        q = q.ApplyOrdering(gq);
        var total = q.LongCount();
        var items = q.Skip((page - 1) * size).Take(size).ToList();
        return Task.FromResult(new PagedResult<T>(items, total, page, size));
    }
}
