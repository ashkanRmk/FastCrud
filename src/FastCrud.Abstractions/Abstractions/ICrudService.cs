using FastCrud.Abstractions.Primitives;
using FastCrud.Abstractions.Query;

namespace FastCrud.Abstractions.Abstractions;

public interface ICrudService<TAgg, in TId>
{
    Task<TAgg> CreateAsync(object input, CancellationToken cancellationToken);

    Task<TAgg?> GetByIdAsync(TId id, CancellationToken cancellationToken);

    Task<PagedResult<TOut>> GetListAsync<TOut>(
        IQuerySpec spec,
        Func<IQueryable<TAgg>, IQueryable<TOut>> projector,
        CancellationToken cancellationToken = default);
    
    Task<TAgg> UpdateAsync(TId id, object input, CancellationToken cancellationToken);

    Task DeleteAsync(TId id, CancellationToken cancellationToken);
}