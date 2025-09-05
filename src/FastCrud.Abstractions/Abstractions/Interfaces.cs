using FastCrud.Abstractions.Primitives;

namespace FastCrud.Abstractions;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

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
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A paged result of aggregates matching the query.</returns>
    Task<PagedResult<TAgg>> QueryAsync(IQuerySpec spec, CancellationToken cancellationToken);

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

/// <summary>
/// Abstraction over an object mapper. Implementations map between DTOs and domain entities.
/// </summary>
public interface IObjectMapper
{
    /// <summary>
    /// Maps an arbitrary source object to a destination type.
    /// </summary>
    /// <typeparam name="TDest">Destination type.</typeparam>
    /// <param name="source">Source object.</param>
    /// <returns>The mapped object.</returns>
    TDest Map<TDest>(object source);

    /// <summary>
    /// Maps an arbitrary source object onto an existing destination instance.
    /// </summary>
    /// <param name="source">Source object.</param>
    /// <param name="destination">Destination object to mutate.</param>
    void Map(object source, object destination);
}

/// <summary>
/// Abstraction over a model validator. Implementations validate domain entities using various validation libraries.
/// </summary>
public interface IModelValidator<T>
{
    /// <summary>
    /// Validates the supplied model and throws an exception if validation fails.
    /// </summary>
    /// <param name="model">Model to validate.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task ValidateAsync(T model, CancellationToken cancellationToken);
}

/// <summary>
/// Abstraction over a query engine. Implementations apply filtering, sorting and paging to LINQ queries.
/// </summary>
public interface IQueryEngine
{
    /// <summary>
    /// Applies the supplied <see cref="IQuerySpec"/> to the query and returns a paged result.
    /// </summary>
    /// <typeparam name="TAgg">Aggregate type.</typeparam>
    /// <param name="query">Queryable.</param>
    /// <param name="spec">Query specification describing paging, filtering and sorting.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task<PagedResult<TAgg>> ApplyQueryAsync<TAgg>(IQueryable<TAgg> query, IQuerySpec spec, CancellationToken cancellationToken);
}

/// <summary>
/// Provides access to the current UTC time. Abstracted for testability.
/// </summary>
public interface IFastCrudClock
{
    /// <summary>
    /// Gets the current UTC time.
    /// </summary>
    DateTime UtcNow { get; }
}

/// <summary>
/// Represents a query specification for paging, filtering and sorting.
/// </summary>
public interface IQuerySpec
{
    /// <summary>
    /// Gets or sets the page index (1-based).
    /// </summary>
    int Page { get; set; }

    /// <summary>
    /// Gets or sets the size of each page.
    /// </summary>
    int PageSize { get; set; }

    /// <summary>
    /// Gets or sets an optional filter string. Implementations may parse this string to construct filter expressions.
    /// </summary>
    string? Filter { get; set; }

    /// <summary>
    /// Gets or sets an optional sort expression. Comma-separated list of property names; prefix with '-' for descending order.
    /// </summary>
    string? Sort { get; set; }
}
