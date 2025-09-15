using System.Collections;
using FastCrud.Abstractions.Abstractions;
using FastCrud.Abstractions.Primitives;
using FastCrud.Abstractions.Query;

namespace FastCrud.Core.Services;

public class CrudService<TAgg, TId, TCreateDto, TUpdateDto>(
        IRepository<TAgg, TId> repository,
        IObjectMapper mapper,
        IEnumerable<IModelValidator<TAgg>> validators,
        IServiceProvider serviceProvider,
        IQueryEngine queryEngine)
        : ICrudService<TAgg, TId, TCreateDto, TUpdateDto>
{
    public async Task<TAgg> CreateAsync(TCreateDto input, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(input);

        await ValidateDtoAsync(input!, serviceProvider, cancellationToken);

        var entity = (TAgg)Activator.CreateInstance(typeof(TAgg), nonPublic: true)!
                    ?? throw new InvalidOperationException(
                        $"{typeof(TAgg).Name} must have a parameterless constructor (can be private).");

        mapper.Map(input, entity);

        await ValidateModelAsync(entity, cancellationToken);
        await repository.AddAsync(entity, cancellationToken);
        await repository.SaveChangesAsync(cancellationToken);
        return entity;
    }

    public async Task DeleteAsync(TId id, CancellationToken cancellationToken)
    {
        var entity = await repository.FindAsync(id, cancellationToken);
        if (entity is null) return;
        await repository.DeleteAsync(entity, cancellationToken);
        await repository.SaveChangesAsync(cancellationToken);
    }

    public async Task<TAgg?> GetByIdAsync(TId id, CancellationToken cancellationToken)
        => await repository.FindAsync(id, cancellationToken);

    public async Task<PagedResult<TOut>> GetListAsync<TOut>(
        IQuerySpec spec,
        Func<IQueryable<TAgg>, IQueryable<TOut>> projector,
        CancellationToken ct = default)
            => await queryEngine.ApplyQueryAsync(repository.Query(), spec, projector, ct);

    public async Task<TAgg> UpdateAsync(TId id, TUpdateDto input, CancellationToken ct)
    {
        ArgumentNullException.ThrowIfNull(input);
        var entity = await repository.FindAsync(id, ct)
                    ?? throw new InvalidOperationException($"{typeof(TAgg).Name} with id {id} not found");

        mapper.Map(input, entity);

        await ValidateModelAsync(entity, ct);
        await repository.SaveChangesAsync(ct);
        return entity;
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



