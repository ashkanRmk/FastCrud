using Crud.Generator.Data;

namespace Crud.Generator.Infrastructure;

[Flags]
public enum CrudOps
{
    None = 0,
    GetAll = 1 << 0,
    GetById = 1 << 1,
    Create = 1 << 2,
    Update = 1 << 3,
    Delete = 1 << 4,
    All = GetAll | GetById | Create | Update | Delete
}

public static class CrudRegistrar
{
    public static void RegisterCrudEndpoints<TEntity>(
        this WebApplication app,
        string routePrefix,
        CrudOps opsToInclude = CrudOps.All)
        where TEntity : class
    {
        var cleanRoute = routePrefix.StartsWith("/") ? routePrefix : "/" + routePrefix;

        if (opsToInclude.HasFlag(CrudOps.GetAll))
        {
            app.MapGet(cleanRoute, async (IRepository<TEntity> repo) =>
                await repo.GetAllAsync())
                .WithOpenApi();
        }

        if (opsToInclude.HasFlag(CrudOps.GetById))
        {
            app.MapGet($"{cleanRoute}/{{id:int}}", async (int id, IRepository<TEntity> repo) =>
                await repo.GetByIdAsync(id) is TEntity entity
                    ? Results.Ok(entity)
                    : Results.NotFound())
                .WithOpenApi();
        }

        if (opsToInclude.HasFlag(CrudOps.Create))
        {
            app.MapPost(cleanRoute, async (TEntity dto, IRepository<TEntity> repo) =>
                Results.Created(cleanRoute, await repo.AddAsync(dto)))
                .WithOpenApi();
        }

        if (opsToInclude.HasFlag(CrudOps.Update))
        {
            app.MapPut($"{cleanRoute}/{{id:int}}", async (int id, TEntity dto, IRepository<TEntity> repo) =>
                await repo.UpdateAsync(id, dto) is TEntity updated
                    ? Results.Ok(updated)
                    : Results.NotFound())
                .WithOpenApi();
        }

        if (opsToInclude.HasFlag(CrudOps.Delete))
        {
            app.MapDelete($"{cleanRoute}/{{id:int}}", async (int id, IRepository<TEntity> repo) =>
                await repo.DeleteAsync(id)
                    ? Results.NoContent()
                    : Results.NotFound())
                .WithOpenApi();
        }
    }
}