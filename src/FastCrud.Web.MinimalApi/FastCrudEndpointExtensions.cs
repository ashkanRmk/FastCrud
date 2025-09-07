using FastCrud.Abstractions.Abstractions;
using FastCrud.Abstractions.Query;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Mapster;

namespace FastCrud.Web.MinimalApi
{
    public static class FastCrudEndpointExtensions
    {
        public static IEndpointRouteBuilder MapFastCrud<TAgg, TId, TCreateDto, TUpdateDto, TReadDto>(
            this IEndpointRouteBuilder builder, 
            string routePrefix,
            string tagName,
            string groupName,
            CrudOps ops = CrudOps.AllOps)
        {
            var prefix = routePrefix.StartsWith('/') ?  routePrefix : $"/{routePrefix}";
            var group = builder.MapGroup(prefix).WithTags(tagName).WithGroupName(groupName);
            
            if (ops.HasFlag(CrudOps.GetList))
            {
                group.MapGet("/", async (
                    [AsParameters] QuerySpec querySpec, 
                    ICrudService<TAgg, TId> svc,
                    CancellationToken ct) =>
                {
                    var page = await svc.GetListAsync<TReadDto>(
                        querySpec,
                        q => q.ProjectToType<TReadDto>(),
                        ct);
                    return Results.Ok(page);
                })
                .WithName($"List{typeof(TAgg).Name}s");
            }

            if (ops.HasFlag(CrudOps.GetById))
            {
                group.MapGet("/{id}", async (
                    TId id,
                    ICrudService<TAgg, TId> svc,
                    CancellationToken ct) =>
                {
                    var entity = await svc.GetByIdAsync(id, ct);
                    return entity is null ? Results.NotFound() : Results.Ok(entity);
                })
                .WithName($"Get{typeof(TAgg).Name}");
            }

            if (ops.HasFlag(CrudOps.Create))
            {
                group.MapPost("/", async (
                    [FromBody] TCreateDto dto,
                    ICrudService<TAgg, TId> svc,
                    CancellationToken ct) =>
                {
                    var created = await svc.CreateAsync(dto!, ct);
                    return Results.Ok(created);
                })
                .WithName($"Create{typeof(TAgg).Name}");
            }

            if (ops.HasFlag(CrudOps.Update))
            {
                group.MapPut("/{id}", async (
                    TId id,
                    [FromBody] TUpdateDto dto,
                    ICrudService<TAgg, TId> svc,
                    CancellationToken ct) =>
                {
                    var updated = await svc.UpdateAsync(id, dto!, ct);
                    return Results.Ok(updated);
                })
                .WithName($"Update{typeof(TAgg).Name}");
            }

            if (ops.HasFlag(CrudOps.Delete))
            {
                group.MapDelete("/{id}", async (
                    TId id,
                    ICrudService<TAgg, TId> svc,
                    CancellationToken ct) =>
                {
                    await svc.DeleteAsync(id, ct);
                    return Results.NoContent();
                })
                .WithName($"Delete{typeof(TAgg).Name}");
            }

            return builder;
        }
    }
}
