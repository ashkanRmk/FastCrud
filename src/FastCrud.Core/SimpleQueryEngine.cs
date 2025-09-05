using System.Linq.Expressions;
using FastCrud.Abstractions.Primitives;
using FastCrud.Abstractions.Query;

namespace FastCrud.Core
{
    /// <summary>
    /// A simple query engine that applies basic filtering, sorting and paging to an <see cref="IQueryable{T}"/>.
    /// This implementation does not support complex filter expressions but demonstrates the extensibility point for FastCrud.
    /// </summary>
    public class SimpleQueryEngine : IQueryEngine
    {
        /// <inheritdoc />
        public Task<PagedResult<TAgg>> ApplyQueryAsync<TAgg>(IQueryable<TAgg> query, IQuerySpec spec, CancellationToken cancellationToken)
        {
            if (spec == null) throw new ArgumentNullException(nameof(spec));

            var q = query;
            // basic equals filtering: "Property=Value" or multiple separated by ';' or ','
            if (!string.IsNullOrWhiteSpace(spec.Filter))
            {
                var filters = spec.Filter.Split(new[] { ';', ',' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (var filter in filters)
                {
                    var parts = filter.Split('=', 2);
                    if (parts.Length == 2)
                    {
                        var propName = parts[0].Trim();
                        var valueStr = parts[1].Trim();
                        q = ApplyEqualsFilter(q, propName, valueStr);
                    }
                }
            }

            // sorting: "Prop" for ascending, "-Prop" for descending, comma-separated
            if (!string.IsNullOrWhiteSpace(spec.Sort))
            {
                bool first = true;
                foreach (var part in spec.Sort.Split(',', StringSplitOptions.RemoveEmptyEntries))
                {
                    var trimmed = part.Trim();
                    if (string.IsNullOrEmpty(trimmed)) continue;
                    bool desc = trimmed.StartsWith("-");
                    var prop = desc ? trimmed.Substring(1) : trimmed;
                    q = ApplyOrderBy(q, prop, desc, first);
                    first = false;
                }
            }

            var total = q.Count();
            var page = spec.Page <= 0 ? 1 : spec.Page;
            var pageSize = spec.PageSize <= 0 ? 50 : spec.PageSize;
            var items = q.Skip((page - 1) * pageSize).Take(pageSize).ToList();

            return Task.FromResult(new PagedResult<TAgg>(items, total, page, pageSize));

            // local functions for filtering and sorting
            static IQueryable<TAgg> ApplyEqualsFilter(IQueryable<TAgg> source, string propertyName, string value)
            {
                var parameter = Expression.Parameter(typeof(TAgg), "x");
                var property = Expression.PropertyOrField(parameter, propertyName);
                var convertedValue = Convert.ChangeType(value, property.Type);
                var constant = Expression.Constant(convertedValue);
                var body = Expression.Equal(property, constant);
                var lambda = Expression.Lambda<Func<TAgg, bool>>(body, parameter);
                return source.Where(lambda);
            }

            static IQueryable<TAgg> ApplyOrderBy(IQueryable<TAgg> source, string propertyName, bool desc, bool first)
            {
                var parameter = Expression.Parameter(typeof(TAgg), "x");
                var property = Expression.PropertyOrField(parameter, propertyName);
                var lambda = Expression.Lambda(property, parameter);
                var methodName = first ? (desc ? "OrderByDescending" : "OrderBy") : (desc ? "ThenByDescending" : "ThenBy");
                var method = typeof(Queryable).GetMethods()
                    .First(m => m.Name == methodName && m.GetParameters().Length == 2)
                    .MakeGenericMethod(typeof(TAgg), property.Type);
                return (IQueryable<TAgg>)method.Invoke(null, new object[] { source, lambda })!;
            }
        }
    }
}