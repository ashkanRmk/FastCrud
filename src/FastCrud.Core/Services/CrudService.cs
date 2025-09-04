using FastCrud.Abstractions;
using FastCrud.Abstractions.Primitives;
using FastCrud.Abstractions.Query;
using Microsoft.Extensions.DependencyInjection;

namespace FastCrud.Core.Services;

public class CrudService<TAgg, TId> : ICrudService<TAgg, TId>
{
    private readonly IRepository<TAgg, TId> _repo;
    private readonly IQueryEngine _queryEngine;
    private readonly IServiceProvider _sp;

    public CrudService(IRepository<TAgg, TId> repo, IQueryEngine queryEngine, IServiceProvider sp)
    {
        _repo = repo;
        _queryEngine = queryEngine;
        _sp = sp;
    }

    public async Task<TAgg> CreateAsync(TAgg entity, CancellationToken ct = default)
    {
        var validator = _sp.GetService<IModelValidator<TAgg>>();
        if (validator is not null)
        {
            var vr = await validator.ValidateAsync(entity, ct);
            if (!vr.IsValid) throw new InvalidOperationException($"Validation failed: {string.Join(',', vr.Errors.Select(e => e.Field+":"+e.Message))}");
        }
        var created = await _repo.AddAsync(entity, ct);
        await _repo.SaveChangesAsync(ct);
        return created;
    }

    public async Task DeleteAsync(TId id, CancellationToken ct = default)
    {
        var entity = await _repo.FindAsync(id, ct) ?? throw new KeyNotFoundException("Not found");
        await _repo.DeleteAsync(entity, ct);
        await _repo.SaveChangesAsync(ct);
    }

    public async Task<TAgg?> GetAsync(TId id, CancellationToken ct = default)
        => await _repo.FindAsync(id, ct);

    public async Task<PagedResult<TAgg>> QueryAsync(IQuerySpec spec, CancellationToken ct = default)
        => await _queryEngine.ApplyAsync(_repo.Query(), spec, ct);

    public async Task<TAgg> UpdateAsync(TId id, object patch, CancellationToken ct = default)
    {
        var entity = await _repo.FindAsync(id, ct) ?? throw new KeyNotFoundException("Not found");
        // Simple map-onto for now (adapter can do better mapping)
        var mapper = _sp.GetService<IObjectMapper>();
        if (mapper is not null)
        {
            var updated = mapper.Map<TAgg>(patch);
            entity = updated;
        }
        entity = await _repo.UpdateAsync(entity, ct);
        await _repo.SaveChangesAsync(ct);
        return entity;
    }
}
