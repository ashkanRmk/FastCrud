using FastCrud.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace FastCrud.Persistence.EFCore;

public class EfRepository<TAgg, TId, TDbContext> : IRepository<TAgg, TId>
    where TAgg : class
    where TDbContext : DbContext
{
    private readonly TDbContext _db;

    public EfRepository(TDbContext db) => _db = db;

    public IQueryable<TAgg> Query() => _db.Set<TAgg>().AsQueryable();

    public async Task<TAgg?> FindAsync(TId id, CancellationToken ct = default)
        => await _db.Set<TAgg>().FindAsync(new object?[] { id! }, ct);

    public async Task<TAgg> AddAsync(TAgg entity, CancellationToken ct = default)
    {
        await _db.Set<TAgg>().AddAsync(entity, ct);
        return entity;
    }

    public Task<TAgg> UpdateAsync(TAgg entity, CancellationToken ct = default)
    {
        _db.Set<TAgg>().Update(entity);
        return Task.FromResult(entity);
    }

    public Task DeleteAsync(TAgg entity, CancellationToken ct = default)
    {
        _db.Set<TAgg>().Remove(entity);
        return Task.CompletedTask;
    }

    public Task<int> SaveChangesAsync(CancellationToken ct = default)
        => _db.SaveChangesAsync(ct);
}
