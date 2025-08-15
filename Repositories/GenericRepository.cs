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
    
    public async Task<TReadDto?> UpdateAsync<TUpdateDto, TReadDto>(TKey id, TUpdateDto dto, CancellationToken ct = default)
    {
        var entity = await db.Set<T>().FindAsync([id], ct);
        if (entity is null) return default;

        dto.Adapt(entity);

        await db.SaveChangesAsync(ct);

        return entity.Adapt<TReadDto>();
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

    public async Task<QueryResult<TDto>> GetAllPaginatedAsync<TDto>(GridifyQuery gridifyQuery, CrudOps crudOps, CancellationToken ct = default)
    {
        var query = db.Set<T>().AsNoTracking().AsQueryable();

        if (crudOps.HasFlag(CrudOps.GetFiltered) && !string.IsNullOrWhiteSpace(gridifyQuery.Filter))
            query = query.ApplyFiltering(gridifyQuery);

        if (crudOps.HasFlag(CrudOps.GetSorted) && !string.IsNullOrWhiteSpace(gridifyQuery.OrderBy))
            query = query.ApplyOrdering(gridifyQuery);

        var total = await query.CountAsync(ct);

        if (crudOps.HasFlag(CrudOps.GetPaginated))
            query = query.ApplyPaging(gridifyQuery);

        var items = await query.ProjectToType<TDto>().ToListAsync(ct);

        return new QueryResult<TDto>
        {
            Items = items,
            TotalItems = total,
            Page = crudOps.HasFlag(CrudOps.GetPaginated) ? gridifyQuery.Page : 1,
            PageSize = crudOps.HasFlag(CrudOps.GetPaginated) ? gridifyQuery.PageSize : items.Count
        };
    }
}