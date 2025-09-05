namespace FastCrud.Abstractions.Abstractions;

/// <summary>
/// Represents a repository abstraction for an aggregate type <typeparamref name="TAgg"/> with identity <typeparamref name="TId"/>.
/// </summary>
public interface IRepository<TAgg, TId>
{
    /// <summary>
    /// Adds a new aggregate instance to the underlying data store.
    /// </summary>
    /// <param name="entity">Aggregate instance to add.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The added aggregate.</returns>
    Task<TAgg> AddAsync(TAgg entity, CancellationToken cancellationToken);

    /// <summary>
    /// Finds an aggregate by its identifier.
    /// </summary>
    /// <param name="id">Identifier of the aggregate.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The aggregate or <c>null</c> if not found.</returns>
    Task<TAgg?> FindAsync(TId id, CancellationToken cancellationToken);

    /// <summary>
    /// Removes an aggregate from the underlying data store.
    /// </summary>
    /// <param name="entity">Aggregate to remove.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task DeleteAsync(TAgg entity, CancellationToken cancellationToken);

    /// <summary>
    /// Returns an <see cref="IQueryable{TAgg}"/> representing the aggregates in the store.
    /// </summary>
    IQueryable<TAgg> Query();

    /// <summary>
    /// Persists changes to the underlying data store.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The number of state entries written to the underlying database.</returns>
    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}