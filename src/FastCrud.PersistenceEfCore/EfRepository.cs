using Microsoft.EntityFrameworkCore;
using FastCrud.Abstractions.Abstractions;

namespace FastCrud.Persistence.EFCore
{
    public class EfRepository<TAgg, TId, TDbContext>(TDbContext context) : IRepository<TAgg, TId>
        where TAgg : class
        where TDbContext : DbContext
    {
        private readonly DbSet<TAgg> _set = context.Set<TAgg>();

        public async Task<TAgg> AddAsync(TAgg entity, CancellationToken cancellationToken)
        {
            await _set.AddAsync(entity, cancellationToken);
            return entity;
        }

        public async Task DeleteAsync(TAgg entity, CancellationToken cancellationToken)
        {
            _set.Remove(entity);
            await Task.CompletedTask;
        }

        public async Task<TAgg?> FindAsync(TId id, CancellationToken cancellationToken)
        {
            return await _set.FindAsync([id], cancellationToken);
        }

        public IQueryable<TAgg> Query() => _set.AsQueryable();

        public Task<int> SaveChangesAsync(CancellationToken cancellationToken)
        {
            return context.SaveChangesAsync(cancellationToken);
        }
    }
}
