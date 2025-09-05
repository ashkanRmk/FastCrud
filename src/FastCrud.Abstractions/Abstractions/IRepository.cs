namespace FastCrud.Abstractions.Abstractions;

public interface IRepository<TAgg, TId>
{
    Task<TAgg> AddAsync(TAgg entity, CancellationToken cancellationToken);
    
    Task<TAgg?> FindAsync(TId id, CancellationToken cancellationToken);

    Task DeleteAsync(TAgg entity, CancellationToken cancellationToken);

    IQueryable<TAgg> Query();

    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}