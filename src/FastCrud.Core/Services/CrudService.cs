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
    IQueryEngine queryEngine,
    IAuditService? auditService = null)
    : ICrudService<TAgg, TId, TCreateDto, TUpdateDto>
{
    public async Task<TAgg> CreateAsync(TCreateDto input, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(input);
        await ValidateDtoAsync(input!, serviceProvider, cancellationToken);

        var entity = mapper.Map<TAgg>(input);
        await ValidateModelAsync(entity, cancellationToken);
        await repository.AddAsync(entity, cancellationToken);
        await repository.SaveChangesAsync(cancellationToken);

        if (auditService != null)
        {
            await auditService.LogAsync(entity, AuditAction.Create, newValues: entity, cancellationToken: cancellationToken);
        }

        return entity;
    }

    public async Task DeleteAsync(TId id, CancellationToken cancellationToken)
    {
        var entity = await repository.FindAsync(id, cancellationToken);
        if (entity is null) return;

        if (auditService != null)
        {
            await auditService.LogAsync(entity, AuditAction.Delete, oldValues: entity, cancellationToken: cancellationToken);
        }

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

        TAgg? oldValues = auditService != null ? CloneEntity(entity) : default(TAgg?);

        mapper.Map(input, entity);
        await ValidateModelAsync(entity, ct);
        await repository.SaveChangesAsync(ct);

        if (auditService != null && oldValues != null)
        {
            await auditService.LogAsync(entity, AuditAction.Update, oldValues: oldValues, newValues: entity, cancellationToken: ct);
        }

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