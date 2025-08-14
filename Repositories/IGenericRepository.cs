namespace Crud.Generator.Repositories;

public interface IGenericRepository<T, TKey> where T : class
{
    Task<List<T>> GetAllAsync(CancellationToken ct = default);
    Task<T?> GetByIdAsync(TKey id, CancellationToken ct = default);
    Task<T> AddAsync(T entity, CancellationToken ct = default);
    Task<T?> UpdateAsync(TKey id, Action<T> update, CancellationToken ct = default);
    Task<bool> DeleteAsync(TKey id, CancellationToken ct = default);
    IQueryable<T> Query();
}