using FastCrud.Abstractions.Primitives;
using FastCrud.Abstractions.Query;

namespace FastCrud.Abstractions.Abstractions;

public interface ICrudService<TAgg, TId, TCreateDto, TUpdateDto>
{
    Task<TAgg> CreateAsync(TCreateDto input, CancellationToken ct);
    Task<TAgg> UpdateAsync(TId id, TUpdateDto input, CancellationToken ct);
    Task DeleteAsync(TId id, CancellationToken ct);
    Task<TAgg?> GetByIdAsync(TId id, CancellationToken ct);
    Task<PagedResult<TOut>> GetListAsync<TOut>(
        IQuerySpec spec,
        Func<IQueryable<TAgg>, IQueryable<TOut>> projector,
        CancellationToken cancellationToken = default);
}