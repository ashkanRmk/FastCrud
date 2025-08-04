using Crud.Generator.Data;

namespace Crud.Generator.Infrastructure;

public static class CrudRegistrar
{
    public static void RegisterCrudEndpoints<TEntity>(
        this WebApplication app,
        CrudOps opsToInclude = CrudOps.All,
        string version = "v1")
        where TEntity : class
    {
        var cleanRoute = "/" + typeof(TEntity).Name.ToLower();

        var group = app
            .MapGroup(version)
            .WithTags(typeof(TEntity).Name);

        if (opsToInclude.HasFlag(CrudOps.GetAll))
        {
            group.MapGet(cleanRoute, async (IRepository<TEntity> repo) =>
                await repo.GetAllAsync())
                .WithOpenApi();
        }

        if (opsToInclude.HasFlag(CrudOps.GetById))
        {
            group.MapGet($"{cleanRoute}/{{id:int}}", async (int id, IRepository<TEntity> repo) =>
                await repo.GetByIdAsync(id) is TEntity entity
                    ? Results.Ok(entity)
                    : Results.NotFound())
                .WithOpenApi();
        }

        if (opsToInclude.HasFlag(CrudOps.Create))
        {
            group.MapPost(cleanRoute, async (TEntity dto, IRepository<TEntity> repo) =>
                Results.Created(cleanRoute, await repo.AddAsync(dto)))
                .WithOpenApi();
        }

        if (opsToInclude.HasFlag(CrudOps.Update))
        {
            group.MapPut($"{cleanRoute}/{{id:int}}", async (int id, TEntity dto, IRepository<TEntity> repo) =>
                await repo.UpdateAsync(id, dto) is TEntity updated
                    ? Results.Ok(updated)
                    : Results.NotFound())
                .WithOpenApi();
        }

        if (opsToInclude.HasFlag(CrudOps.Delete))
        {
            group.MapDelete($"{cleanRoute}/{{id:int}}", async (int id, IRepository<TEntity> repo) =>
                await repo.DeleteAsync(id)
                    ? Results.NoContent()
                    : Results.NotFound())
                .WithOpenApi();
        }
    }
}