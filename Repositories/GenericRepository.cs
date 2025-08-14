using FastCrud.Data;
using FastCrud.Infrastructure;
using Gridify;
using Mapster;
using Microsoft.EntityFrameworkCore;

namespace FastCrud.Repositories;

public class GenericRepository<T, TKey>(AppDbContext db) : IGenericRepository<T, TKey>
    where T : class, Abstractions.IEntity<TKey>
{
    public async Task<List<TDto>> GetAllAsync<TDto>(CancellationToken ct = default)
        => await db.Set<T>()
        .AsNoTracking()
        .ProjectToType<TDto>()
        .ToListAsync(ct);


    public async Task<TDto?> GetByIdAsync<TDto>(TKey id, CancellationToken ct = default)
        => await db.Set<T>()
            .AsNoTracking()
            .Where(e => EqualityComparer<TKey>.Default.Equals(e.Id, id))
            .ProjectToType<TDto>()
            .FirstOrDefaultAsync(ct);


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

    public async Task<QueryResult<TDto>> GetAllPaginatedAsync<TDto>(GridifyQuery gq, CrudOps crudOps, CancellationToken ct = default)
    {
        var q = db.Set<T>().AsNoTracking().AsQueryable();

        if (crudOps.HasFlag(CrudOps.GetFiltered) && !string.IsNullOrWhiteSpace(gq.Filter))
            q = q.ApplyFiltering(gq);

        if (crudOps.HasFlag(CrudOps.GetSorted) && !string.IsNullOrWhiteSpace(gq.OrderBy))
            q = q.ApplyOrdering(gq);

        var total = await q.CountAsync(ct);

        if (crudOps.HasFlag(CrudOps.GetPaginated))
            q = q.ApplyPaging(gq);

        var items = await q.ProjectToType<TDto>().ToListAsync(ct);

        return new QueryResult<TDto>
        {
            Items = items,
            TotalItems = total,
            Page = crudOps.HasFlag(CrudOps.GetPaginated) ? gq.Page : 1,
            PageSize = crudOps.HasFlag(CrudOps.GetPaginated) ? gq.PageSize : items.Count
        };
    }
}