using FastCrud.Infrastructure;
using Gridify;

namespace FastCrud.Repositories;

public interface IGenericRepository<T, TKey> where T : class, Abstractions.IEntity<TKey>
{
    Task<List<TDto>> GetAllAsync<TDto>(CancellationToken ct = default);
    Task<TDto?> GetByIdAsync<TDto>(TKey id, CancellationToken ct = default);
    Task<T> AddAsync(T entity, CancellationToken ct = default);
    Task<T?> UpdateAsync(TKey id, Action<T> update, CancellationToken ct = default);
    Task<bool> DeleteAsync(TKey id, CancellationToken ct = default);
    IQueryable<T> Query();

    Task<QueryResult<TDto>> GetAllPaginatedAsync<TDto>(GridifyQuery gq, CrudOps crudOps, CancellationToken ct = default);

}