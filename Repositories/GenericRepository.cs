using FastCrud.Data;
using FastCrud.Infrastructure;
using Gridify;
using Mapster;
using Microsoft.EntityFrameworkCore;

namespace FastCrud.Repositories;

public class GenericRepository<T, TKey> : IGenericRepository<T, TKey>
    where T : class, Abstractions.IEntity<TKey>
{
    private readonly AppDbContext _db;
    private readonly GridifyMapper<T>? _mapper;

    public GenericRepository(AppDbContext db, GridifyMapper<T>? mapper = null)
    {
        _db = db;
        _mapper = mapper;
    }

    public async Task<List<TDto>> GetAllAsync<TDto>(CancellationToken ct = default)
        => await _db.Set<T>()
            .AsNoTracking()
            .ProjectToType<TDto>()
            .ToListAsync(ct);

    public async Task<TDto?> GetByIdAsync<TDto>(TKey id, CancellationToken ct = default)
        => await _db.Set<T>()
            .AsNoTracking()
            .Where(e => EqualityComparer<TKey>.Default.Equals(e.Id, id))
            .ProjectToType<TDto>()
            .FirstOrDefaultAsync(ct);

    public async Task<T> AddAsync(T entity, CancellationToken ct = default)
    {
        _db.Set<T>().Add(entity);
        await _db.SaveChangesAsync(ct);
        return entity;
    }

    public async Task<TReadDto?> UpdateAsync<TUpdateDto, TReadDto>(TKey id, TUpdateDto dto,
        CancellationToken ct = default)
    {
        var entity = await _db.Set<T>().FindAsync([id], ct);
        if (entity is null) return default;

        dto.Adapt(entity);

        await _db.SaveChangesAsync(ct);

        return entity.Adapt<TReadDto>();
    }

    public async Task<bool> DeleteAsync(TKey id, CancellationToken ct = default)
    {
        var entity = await _db.Set<T>().FindAsync([id], ct);
        if (entity is null) return false;

        _db.Remove(entity);

        await _db.SaveChangesAsync(ct);
        return true;
    }

    public IQueryable<T> Query() => _db.Set<T>().AsQueryable();

    public async Task<QueryResult<TDto>> GetAllPaginatedAsync<TDto>(GridifyQuery gridifyQuery, CrudOps crudOps,
        CancellationToken ct = default)
    {
        var query = _db.Set<T>().AsNoTracking().AsQueryable();

        try
        {
            if (crudOps.HasFlag(CrudOps.GetFiltered) && !string.IsNullOrWhiteSpace(gridifyQuery.Filter))
                query = _mapper is null ? query.ApplyFiltering(gridifyQuery) : query.ApplyFiltering(gridifyQuery, _mapper);

            if (crudOps.HasFlag(CrudOps.GetSorted) && !string.IsNullOrWhiteSpace(gridifyQuery.OrderBy))
                query = _mapper is null ? query.ApplyOrdering(gridifyQuery) : query.ApplyOrdering(gridifyQuery, _mapper);
        }
        catch (Exception ex) when (ex.GetType().Name.Contains("Gridify", StringComparison.OrdinalIgnoreCase))
        {
            throw new BadHttpRequestException("Unsupported filter/sort field. Check allowed fields.", 400);
        }

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