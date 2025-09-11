using FastCrud.Abstractions.Abstractions;
using FastCrud.Abstractions.Primitives;
using FastCrud.Abstractions.Query;
using System.Collections;

namespace FastCrud.Core.Services
{
    public class CrudService<TAgg, TId>(
        IRepository<TAgg, TId> repository,
        IObjectMapper mapper,
        IEnumerable<IModelValidator<TAgg>> validators,
        IServiceProvider serviceProvider,
        IQueryEngine queryEngine,
        IAuditService? auditService = null)
        : ICrudService<TAgg, TId>
    {
        public async Task<TAgg> CreateAsync(object input, CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(input);

            await ValidateDtoAsync(input, serviceProvider, cancellationToken);

            var entity = input is TAgg agg ? agg : mapper.Map<TAgg>(input);

            await ValidateModelAsync(entity, cancellationToken);

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

            TAgg? oldValues = auditService != null ? CloneEntity(entity) : default(TAgg?);

            if (input is TAgg agg)
            {
                mapper.Map(agg, entity);
            }
            else
            {
                mapper.Map(input, entity);
            }

            await ValidateModelAsync(entity, cancellationToken);

            await repository.SaveChangesAsync(cancellationToken);

            return entity;
        }

        private TAgg? CloneEntity(TAgg entity)
        {
            try
            {
                return mapper.Map<TAgg>(entity);
            }
            catch
            {
                return default(TAgg?);
            }
        }

        private async Task ValidateModelAsync(
            TAgg entity,
            CancellationToken cancellationToken)
        {
            foreach (var validator in validators)
            {
                await validator.ValidateAsync(entity, cancellationToken);
            }
        }

        private static async Task ValidateDtoAsync(
            object dto,
            IServiceProvider serviceProvider,
            CancellationToken cancellationToken)
        {
            var dtoType = dto.GetType();
            var validatorInterface = typeof(IModelValidator<>).MakeGenericType(dtoType);
            var enumerableType = typeof(IEnumerable<>).MakeGenericType(validatorInterface);

            if (serviceProvider.GetService(enumerableType) is not IEnumerable validators)
                return;

            foreach (var v in validators)
            {
                var method = v.GetType().GetMethod("ValidateAsync", [dtoType, typeof(CancellationToken)])!;
                var task = (Task)method.Invoke(v, [dto, cancellationToken])!;
                await task.ConfigureAwait(false);
            }
        }
    }
}