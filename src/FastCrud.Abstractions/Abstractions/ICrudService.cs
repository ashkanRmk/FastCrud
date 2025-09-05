using FastCrud.Abstractions.Primitives;
using FastCrud.Abstractions.Query;

namespace FastCrud.Abstractions.Abstractions;

/// <summary>
/// Represents a generic CRUD service over an aggregate type <typeparamref name="TAgg"/> with identity <typeparamref name="TId"/>.
/// Implementations should handle mapping, validation, querying and persistence via registered components.
/// </summary>
public interface ICrudService<TAgg, TId>
{
    /// <summary>
    /// Creates a new instance of <typeparamref name="TAgg"/> from an arbitrary input object. The input may be the entity itself
    /// or a DTO. Implementations should map and validate as appropriate.
    /// </summary>
    /// <param name="input">An object to map onto a new aggregate instance.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The created aggregate.</returns>
    Task<TAgg> CreateAsync(object input, CancellationToken cancellationToken);

    /// <summary>
    /// Retrieves an aggregate by its identifier.
    /// </summary>
    /// <param name="id">Identifier of the aggregate.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The aggregate or <c>null</c> if not found.</returns>
    Task<TAgg?> GetAsync(TId id, CancellationToken cancellationToken);

    /// <summary>
    /// Queries aggregates using a generic <see cref="IQuerySpec"/> describing paging, filtering and sorting information.
    /// </summary>
    /// <param name="spec">Query specification.</param>
    /// <param name="projector"></param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A paged result of aggregates matching the query.</returns>
    Task<PagedResult<TOut>> QueryAsync<TOut>(
        IQuerySpec spec,
        Func<IQueryable<TAgg>, IQueryable<TOut>> projector,
        CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Updates an aggregate by its identifier from an arbitrary input object. The input may be a DTO or the entity itself.
    /// Implementations should map and validate as appropriate.
    /// </summary>
    /// <param name="id">Identifier of the aggregate to update.</param>
    /// <param name="input">An object to map onto the existing aggregate.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The updated aggregate.</returns>
    Task<TAgg> UpdateAsync(TId id, object input, CancellationToken cancellationToken);

    /// <summary>
    /// Deletes an aggregate by its identifier.
    /// </summary>
    /// <param name="id">Identifier of the aggregate to delete.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task DeleteAsync(TId id, CancellationToken cancellationToken);
}