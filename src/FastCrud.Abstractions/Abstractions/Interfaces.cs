using FastCrud.Abstractions.Primitives;
using FastCrud.Abstractions.Query;

namespace FastCrud.Abstractions;

public interface ICrudService<TAgg, TId>
{
    Task<TAgg> CreateAsync(TAgg entity, CancellationToken ct = default);
    Task<TAgg?> GetAsync(TId id, CancellationToken ct = default);
    Task<PagedResult<TAgg>> QueryAsync(IQuerySpec spec, CancellationToken ct = default);
    Task<TAgg> UpdateAsync(TId id, object patch, CancellationToken ct = default);
    Task DeleteAsync(TId id, CancellationToken ct = default);
}

public interface IRepository<TAgg, TId>
{
    IQueryable<TAgg> Query();
    Task<TAgg?> FindAsync(TId id, CancellationToken ct = default);
    Task<TAgg> AddAsync(TAgg entity, CancellationToken ct = default);
    Task<TAgg> UpdateAsync(TAgg entity, CancellationToken ct = default);
    Task DeleteAsync(TAgg entity, CancellationToken ct = default);
    Task<int> SaveChangesAsync(CancellationToken ct = default);
}

public interface IObjectMapper
{
    TDest Map<TDest>(object source);
}

public interface IModelValidator<T>
{
    Task<Primitives.ValidationResult> ValidateAsync(T instance, CancellationToken ct = default);
}

public interface IQueryEngine
{
    Task<PagedResult<T>> ApplyAsync<T>(IQueryable<T> query, IQuerySpec spec, CancellationToken ct = default);
}

public interface IFastCrudClock
{
    DateTime UtcNow { get; }
}
