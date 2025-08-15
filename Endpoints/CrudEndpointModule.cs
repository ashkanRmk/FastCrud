using FastCrud.Abstractions;
using FastCrud.Infrastructure;
using FastCrud.Repositories;
using Gridify;
using Gridify.EntityFramework;
using Mapster;
using Microsoft.EntityFrameworkCore;

namespace FastCrud.Endpoints;

public abstract class CrudEndpointModule<T, TKey, TReadDto, TCreateDto, TUpdateDto>
    (CrudOps ops) :
    EndpointModuleBase
    where T : class, IEntity<TKey>, new()
{
    protected override void MapEndpoints(RouteGroupBuilder group)
    {
        if (ops.HasFlag(CrudOps.GetAll))
        {
            group.MapGet("/List", async (IGenericRepository<T, TKey> repo, CancellationToken ct) =>
                {
                    var list = await repo.GetAllAsync<TReadDto>(ct);
                    return Results.Ok(list);
                })
                .WithSummary($"List {typeof(T).Name}")
                .Produces(StatusCodes.Status200OK);
        }

        if (ops.HasFlag(CrudOps.GetFullOpsList))
        {
            group.MapGet("/Paginated", async (IGenericRepository<T, TKey> repo, [AsParameters] GridifyQuery gq, CancellationToken ct) =>
                {
                    try
                    {
                        var list = await repo.GetAllPaginatedAsync<TReadDto>(gq, ops, ct);
                        return Results.Ok(list);
                    }
                    catch (BadHttpRequestException ex)
                    {
                        return Results.BadRequest(ex.Message);
                    }
                })
                .WithSummary($"List {typeof(T).Name} with filtering/sorting/paging")
                .Produces(StatusCodes.Status200OK);
        }

        if (ops.HasFlag(CrudOps.GetById))
        {
            group.MapGet("/{id}", async (IGenericRepository<T, TKey> repo, TKey id, CancellationToken ct) =>
            {
                var dto = await repo.GetByIdAsync<TReadDto>(id, ct);
                return dto is null ? Results.NotFound() : Results.Ok(dto);
            })
            .WithSummary($"Get {typeof(T).Name} by id")
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound);
        }

        if (ops.HasFlag(CrudOps.Create))
        {
            group.MapPost("/", async (IGenericRepository<T, TKey> repo, TCreateDto dto, CancellationToken ct) =>
            {
                var entity = dto.Adapt<T>();
                var created = await repo.AddAsync(entity, ct);
                return Results.Created("/{created.Id}", created.Adapt<TReadDto>());
            })
            .WithSummary($"Create {typeof(T).Name}")
            .Produces(StatusCodes.Status201Created)
            .Produces(StatusCodes.Status400BadRequest);
        }

        if (ops.HasFlag(CrudOps.Update))
        {
            group.MapPut("/{id}", async (IGenericRepository<T, TKey> repo, TKey id, TUpdateDto dto, CancellationToken ct) =>
            {
                var readDto = await repo.UpdateAsync<TUpdateDto, TReadDto>(id, dto, ct);
                return readDto is null ? Results.NotFound() : Results.Ok(readDto);
            })
            .WithSummary($"Update {typeof(T).Name}")
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status400BadRequest);
        }

        if (ops.HasFlag(CrudOps.Delete))
        {
            group.MapDelete("/{id}", async (IGenericRepository<T, TKey> repo, TKey id, CancellationToken ct) =>
            {
                var ok = await repo.DeleteAsync(id, ct);
                return ok ? Results.NoContent() : Results.NotFound();
            })
            .WithSummary($"Delete {typeof(T).Name}")
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status404NotFound);
        }

        MapCustomEndpoints(group);
    }

    protected virtual void MapCustomEndpoints(RouteGroupBuilder group) { }

}

