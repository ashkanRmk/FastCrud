using Crud.Generator.Data;
using Microsoft.EntityFrameworkCore;

namespace Crud.Generator.Repositories;

public class GenericRepository<T, TKey>(AppDbContext db) : IGenericRepository<T, TKey>
    where T : class
{
    public async Task<List<T>> GetAllAsync(CancellationToken ct = default)
        => await db.Set<T>().AsNoTracking().ToListAsync(ct);

    public async Task<T?> GetByIdAsync(TKey id, CancellationToken ct = default)
        => await db.Set<T>().FindAsync([id], ct);

    public async Task<T> AddAsync(T entity, CancellationToken ct = default)
    {
        db.Set<T>().Add(entity);
        await db.SaveChangesAsync(ct);
        return entity;
    }

    public async Task<T?> UpdateAsync(TKey id, Action<T> update, CancellationToken ct = default)
    {
        var entity = await db.Set<T>().FindAsync([id], ct);
        if (entity is null) return null;
        update(entity);
        await db.SaveChangesAsync(ct);
        return entity;
    }

    public async Task<bool> DeleteAsync(TKey id, CancellationToken ct = default)
    {
        var entity = await db.Set<T>().FindAsync([id], ct);
        if (entity is null) return false;
        db.Remove(entity);
        await db.SaveChangesAsync(ct);
        return true;
    }

    public IQueryable<T> Query() => db.Set<T>().AsQueryable();
}