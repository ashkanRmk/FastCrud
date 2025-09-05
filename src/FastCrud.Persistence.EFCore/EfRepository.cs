using Microsoft.EntityFrameworkCore;
using FastCrud.Abstractions.Abstractions;

namespace FastCrud.Persistence.EFCore
{
    /// <summary>
    /// Entity Framework Core implementation of <see cref="IRepository{TAgg, TId}"/>. Wraps a <see cref="DbContext"/> and
    /// exposes common data access operations.
    /// </summary>
    /// <typeparam name="TAgg">Aggregate type.</typeparam>
    /// <typeparam name="TId">Identifier type.</typeparam>
    /// <typeparam name="TDbContext">DbContext type.</typeparam>
    public class EfRepository<TAgg, TId, TDbContext> : IRepository<TAgg, TId>
        where TAgg : class
        where TDbContext : DbContext
    {
        private readonly TDbContext _context;
        private readonly DbSet<TAgg> _set;

        /// <summary>
        /// Initializes a new repository instance.
        /// </summary>
        /// <param name="context">The Entity Framework Core DbContext.</param>
        public EfRepository(TDbContext context)
        {
            _context = context;
            _set = context.Set<TAgg>();
        }

        /// <inheritdoc />
        public async Task<TAgg> AddAsync(TAgg entity, CancellationToken cancellationToken)
        {
            await _set.AddAsync(entity, cancellationToken);
            return entity;
        }

        /// <inheritdoc />
        public async Task DeleteAsync(TAgg entity, CancellationToken cancellationToken)
        {
            _set.Remove(entity);
            await Task.CompletedTask;
        }

        /// <inheritdoc />
        public async Task<TAgg?> FindAsync(TId id, CancellationToken cancellationToken)
        {
            return await _set.FindAsync([id], cancellationToken);
        }

        /// <inheritdoc />
        public IQueryable<TAgg> Query() => _set.AsQueryable();

        /// <inheritdoc />
        public Task<int> SaveChangesAsync(CancellationToken cancellationToken)
        {
            return _context.SaveChangesAsync(cancellationToken);
        }
    }
}
