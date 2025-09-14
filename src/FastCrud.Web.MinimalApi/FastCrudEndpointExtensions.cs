using FastCrud.Abstractions.Abstractions;
using FastCrud.Abstractions.Query;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Mapster;

namespace FastCrud.Web.MinimalApi;

public static class FastCrudEndpointExtensions
{
    public static IEndpointRouteBuilder MapFastCrud<TAgg, TId, TCreateDto, TUpdateDto, TReadDto>(
        this IEndpointRouteBuilder builder,
        string routePrefix,
        CrudOps ops = CrudOps.AllOps,
        string? tagName = null,
        string? groupName = null)
    {
        tagName ??= $"{typeof(TAgg).Name}s";
        var prefix = routePrefix.StartsWith('/') ? routePrefix : $"/{routePrefix}";
        var group = builder.MapGroup(prefix).WithTags(tagName);
        if (groupName is not null) group.WithGroupName(groupName);

        if (ops.HasFlag(CrudOps.GetList))
        {
            group.MapGet("/", async (
                [AsParameters] QuerySpec querySpec,
                ICrudService<TAgg, TId, TCreateDto, TUpdateDto> svc,
                CancellationToken cancellationToken) =>
            {
                var page = await svc.GetListAsync<TReadDto>(
                    querySpec,
                    q => q.ProjectToType<TReadDto>(),
                    cancellationToken);
                return Results.Ok(page);
            })
            .WithName($"List{typeof(TAgg).Name}s");
        }

        if (ops.HasFlag(CrudOps.GetById))
        {
            group.MapGet("/{id}", async (
                TId id,
                ICrudService<TAgg, TId, TCreateDto, TUpdateDto> svc,
                CancellationToken cancellationToken) =>
            {
                var entity = await svc.GetByIdAsync(id, cancellationToken);
                return entity is null ? Results.NotFound() : Results.Ok(entity);
            })
            .WithName($"Get{typeof(TAgg).Name}");
        }

        if (ops.HasFlag(CrudOps.Create))
        {
            group.MapPost("/", async (
                [FromBody] TCreateDto dto,
                ICrudService<TAgg, TId, TCreateDto, TUpdateDto> svc,
                CancellationToken cancellationToken) =>
            {
                var created = await svc.CreateAsync(dto, cancellationToken);
                return Results.Ok(created);
            })
            .WithName($"Create{typeof(TAgg).Name}");
        }

        if (ops.HasFlag(CrudOps.Update))
        {
            group.MapPut("/{id}", async (
                TId id,
                [FromBody] TUpdateDto dto,
                ICrudService<TAgg, TId, TCreateDto, TUpdateDto> svc,
                CancellationToken cancellationToken) =>
            {
                var updated = await svc.UpdateAsync(id, dto, cancellationToken);
                return Results.Ok(updated);
            })
            .WithName($"Update{typeof(TAgg).Name}");
        }

        if (ops.HasFlag(CrudOps.Delete))
        {
            group.MapDelete("/{id}", async (
                TId id,
                ICrudService<TAgg, TId, TCreateDto, TUpdateDto> svc,
                CancellationToken cancellationToken) =>
            {
                await svc.DeleteAsync(id, cancellationToken);
                return Results.NoContent();
            })
            .WithName($"Delete{typeof(TAgg).Name}");
        }

        return builder;
    }
}
