using FastCrud.Abstractions.Abstractions;
using FastCrud.Abstractions.Primitives;
using FastCrud.Abstractions.Query;
using System.Collections;

namespace FastCrud.Core.Services;

public class CrudService<TAgg, TId, TCreateDto, TUpdateDto>(
    IRepository<TAgg, TId> repository,
    IObjectMapper mapper,
    IEnumerable<IModelValidator<TAgg>> validators,
    IServiceProvider serviceProvider,
    IQueryEngine queryEngine)
    : ICrudService<TAgg, TId, TCreateDto, TUpdateDto>
{
    public async Task<OpResult<TAgg>> CreateAsync(TCreateDto input, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(input);

        var (ok, message) = await ValidateDtoAsync(input!, serviceProvider, cancellationToken);
        if (!ok) return new OpResult<TAgg>(false, message);

        var entity = mapper.Map<TAgg>(input);

        var (modelOk, modelMessage) = await ValidateModelAsync(entity, cancellationToken);
        if (!modelOk) return new OpResult<TAgg>(false, modelMessage);

        await repository.AddAsync(entity, cancellationToken);
        await repository.SaveChangesAsync(cancellationToken);

        return new OpResult<TAgg>(true, string.Empty, entity);
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

    public async Task<OpResult<TAgg>> UpdateAsync(TId id, TUpdateDto input, CancellationToken ct)
    {
        ArgumentNullException.ThrowIfNull(input);
        var entity = await repository.FindAsync(id, ct)
                    ?? throw new InvalidOperationException($"{typeof(TAgg).Name} with id {id} not found");

        mapper.Map(input, entity);

        var (ok, message) = await ValidateModelAsync(entity, ct);
        if (!ok) return new OpResult<TAgg>(false, message);

        await repository.SaveChangesAsync(ct);

        return new OpResult<TAgg>(true, string.Empty, entity);
    }

    private async Task<(bool ok, string message)> ValidateModelAsync(
        TAgg entity,
        CancellationToken cancellationToken)
    {
        foreach (var validator in validators)
        {
            var (ok, message) = await validator.ValidateAsync(entity, cancellationToken);
            if (!ok) return (false, message);
        }

        return (true, string.Empty);
    }

    private static async Task<(bool ok, string message)> ValidateDtoAsync(
        object dto,
        IServiceProvider serviceProvider,
        CancellationToken cancellationToken)
    {
        var dtoType = dto.GetType();
        var validatorInterface = typeof(IModelValidator<>).MakeGenericType(dtoType);
        var enumerableType = typeof(IEnumerable<>).MakeGenericType(validatorInterface);

        if (serviceProvider.GetService(enumerableType) is not IEnumerable validators)
            return (true, string.Empty);

        foreach (var v in validators)
        {
            var method = v.GetType().GetMethod("ValidateAsync", [dtoType, typeof(CancellationToken)])!;
            var task = (Task<(bool ok, string message)>)method.Invoke(v, [dto, cancellationToken])!;
            var result = await task.ConfigureAwait(false);
            if (!result.ok) return (false, result.message);
        }
        return (true, string.Empty);
    }
}
