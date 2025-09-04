using FastCrud.Abstractions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

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
            return await _set.FindAsync(new object[] { id }, cancellationToken);
        }

        /// <inheritdoc />
        public IQueryable<TAgg> Query() => _set.AsQueryable();

        /// <inheritdoc />
        public Task<int> SaveChangesAsync(CancellationToken cancellationToken)
        {
            return _context.SaveChangesAsync(cancellationToken);
        }
    }

    /// <summary>
    /// Extensions for registering EF repositories with the DI container.
    /// </summary>
    public static class EfRepositoryServiceCollectionExtensions
    {
        /// <summary>
        /// Registers an EF repository for the specified aggregate type and database context. When invoked, resolves
        /// <see cref="IRepository{TAgg, TId}"/> to an <see cref="EfRepository{TAgg, TId, TDbContext}"/>.
        /// </summary>
        /// <typeparam name="TAgg">Aggregate type.</typeparam>
        /// <typeparam name="TId">Identifier type.</typeparam>
        /// <typeparam name="TDbContext">DbContext type.</typeparam>
        /// <param name="services">The service collection.</param>
        /// <returns>The service collection for chaining.</returns>
        public static IServiceCollection AddEfRepository<TAgg, TId, TDbContext>(this IServiceCollection services)
            where TDbContext : DbContext
        {
            services.AddScoped<IRepository<TAgg, TId>>(sp =>
            {
                var db = sp.GetRequiredService<TDbContext>();
                return new EfRepository<TAgg, TId, TDbContext>(db);
            });
            return services;
        }
    }
}
