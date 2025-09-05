using FastCrud.Abstractions.Abstractions;
using FastCrud.Abstractions.Primitives;
using FastCrud.Abstractions.Query;

namespace FastCrud.Core.Services
{
    public class CrudService<TAgg, TId>(
        IRepository<TAgg, TId> repository,
        IObjectMapper mapper,
        IEnumerable<IModelValidator<TAgg>> validators,
        IQueryEngine queryEngine)
        : ICrudService<TAgg, TId>
    {
        public async Task<TAgg> CreateAsync(object input, CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(input);
            var entity = input is TAgg agg ? agg : mapper.Map<TAgg>(input);
            
            foreach (var validator in validators)
            {
                await validator.ValidateAsync(entity, cancellationToken);
            }
            
            await repository.AddAsync(entity, cancellationToken);
            await repository.SaveChangesAsync(cancellationToken);
            return entity;
        }

        public async Task DeleteAsync(TId id, CancellationToken cancellationToken)
        {
            var entity = await repository.FindAsync(id, cancellationToken);
            if (entity != null)
            {
                await repository.DeleteAsync(entity, cancellationToken);
                await repository.SaveChangesAsync(cancellationToken);
            }
        }

        public async Task<TAgg?> GetByIdAsync(TId id, CancellationToken cancellationToken)
        {
            return await repository.FindAsync(id, cancellationToken);
        }

        public async Task<PagedResult<TOut>> GetListAsync<TOut>(
            IQuerySpec spec,
            Func<IQueryable<TAgg>, IQueryable<TOut>> projector,
            CancellationToken ct = default)
        {
            var q = repository.Query();
            return await queryEngine.ApplyQueryAsync(q, spec, projector, ct);
        }
        
        public async Task<TAgg> UpdateAsync(TId id, object input, CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(input);
            var entity = await repository.FindAsync(id, cancellationToken);
            if (entity == null)
            {
                throw new InvalidOperationException($"{typeof(TAgg).Name} with id {id} not found");
            }
            
            if (input is TAgg agg)
            {
                mapper.Map(agg, entity);
            }
            else
            {
                mapper.Map(input, entity);
            }
            
            foreach (var validator in validators)
            {
                await validator.ValidateAsync(entity, cancellationToken);
            }
            
            await repository.SaveChangesAsync(cancellationToken);
            return entity;
        }
    }
}
