using Crud.Generator.Abstractions;
using Crud.Generator.Repositories;
using Gridify;
using Gridify.EntityFramework;
using Mapster;
using Microsoft.EntityFrameworkCore;
using SharpGrip.FluentValidation.AutoValidation.Endpoints.Extensions;

namespace Crud.Generator.Crud;

public static class CrudEndpointExtensions
{
    public static RouteGroupBuilder MapCrudEndpoints<T, TKey, TReadDto, TCreateDto, TUpdateDto>(
        this IEndpointRouteBuilder app,
        string routePrefix,
        CrudOps ops = CrudOps.All,
        string version = "v1")
        where T : class, IEntity<TKey>, new()
    {
        var cleanRoute = "/" + typeof(T).Name.ToLower();

        var group = app
            .MapGroup(version)
            .WithTags(typeof(T).Name)
            .AddFluentValidationAutoValidation();
        
        if (ops.HasFlag(CrudOps.GetAll))
        {
            group.MapGet(cleanRoute, async (IGenericRepository<T, TKey> repo,[AsParameters] GridifyQuery gq, CancellationToken ct) =>
                {
                    var query = repo.Query().AsNoTracking();
                    
                    var page = await query.GridifyAsync(gq, ct);
                    
                    var items = page.Data.Select(e => e.Adapt<TReadDto>()).ToList();

                    return Results.Ok(new
                    {
                        Page = gq.Page,
                        PageSize = gq.PageSize,
                        TotalItems = page.Count,
                        Items = items
                    });
                })
                .WithSummary($"List {typeof(T).Name} with filtering/sorting/paging")
                .Produces(StatusCodes.Status200OK);
        }

        if (ops.HasFlag(CrudOps.GetById))
        {
            group.MapGet($"{cleanRoute}/{{id}}", async (IGenericRepository<T, TKey> repo, TKey id, CancellationToken ct) =>
            {
                var entity = await repo.GetByIdAsync(id, ct);
                return entity is null ? Results.NotFound() : Results.Ok(entity.Adapt<TReadDto>());
            })
            .WithSummary($"Get {typeof(T).Name} by id")
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound);
        }

        if (ops.HasFlag(CrudOps.Create))
        {
            group.MapPost(cleanRoute, async (IGenericRepository<T, TKey> repo, TCreateDto dto, CancellationToken ct) =>
            {
                var entity = dto.Adapt<T>();
                var created = await repo.AddAsync(entity, ct);
                return Results.Created($"{routePrefix}/{created.Id}", created.Adapt<TReadDto>());
            })
            .WithSummary($"Create {typeof(T).Name}")
            .Produces(StatusCodes.Status201Created)
            .Produces(StatusCodes.Status400BadRequest);
        }

        if (ops.HasFlag(CrudOps.Update))
        {
            group.MapPut($"{cleanRoute}/{{id}}", async (IGenericRepository<T, TKey> repo, TKey id, TUpdateDto dto, CancellationToken ct) =>
            {
                var updated = await repo.UpdateAsync(id, ent => dto.Adapt(ent), ct);
                return updated is null ? Results.NotFound() : Results.Ok(updated.Adapt<TReadDto>());
            })
            .WithSummary($"Update {typeof(T).Name}")
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status400BadRequest);
        }

        if (ops.HasFlag(CrudOps.Delete))
        {
            group.MapDelete($"{cleanRoute}/{{id}}", async (IGenericRepository<T, TKey> repo, TKey id, CancellationToken ct) =>
            {
                var ok = await repo.DeleteAsync(id, ct);
                return ok ? Results.NoContent() : Results.NotFound();
            })
            .WithSummary($"Delete {typeof(T).Name}")
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status404NotFound);
        }

        return group;
    }
}
