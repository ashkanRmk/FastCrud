using FastCrud.Abstractions.Abstractions;
using FastCrud.Abstractions.Query;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Mapster;

namespace FastCrud.Web.MinimalApi
{
    /// <summary>
    /// Extension methods for mapping CRUD endpoints for a given aggregate type.
    /// </summary>
    public static class FastCrudEndpointExtensions
    {
        /// <summary>
        /// Maps CRUD endpoints for the specified aggregate and identifier types. Endpoints are grouped under the supplied route prefix.
        /// </summary>
        /// <typeparam name="TAgg">Aggregate type.</typeparam>
        /// <typeparam name="TId">Identifier type.</typeparam>
        /// <typeparam name="TReadDto">Dto for querying data</typeparam>
        /// <typeparam name="TUpdateDto">DTO for update an aggregate</typeparam>
        /// <typeparam name="TCreateDto">DTO for creating new aggregate</typeparam>
        /// <param name="builder">Endpoint route builder.</param>
        /// <param name="routePrefix">Route prefix (e.g., "/api/orders").</param>
        /// <returns>The route builder for chaining.</returns>
        public static IEndpointRouteBuilder MapFastCrud<TAgg, TId, TCreateDto, TUpdateDto, TReadDto>(
            this IEndpointRouteBuilder builder, string routePrefix)
        {
            var prefix = routePrefix.StartsWith('/') ?  routePrefix : $"/{routePrefix}";
            var group = builder.MapGroup(prefix).WithTags($"{typeof(TAgg).Name}s");
            
            // list/query
            group.MapGet("/", async (
                    [AsParameters] QuerySpec spec, 
                    ICrudService<TAgg, TId> svc,
                    CancellationToken ct) =>
            {
                var page = await svc.QueryAsync<TReadDto>(spec, q => q.ProjectToType<TReadDto>(), ct);
                return Results.Ok(page);
            })
            .WithName($"List{typeof(TAgg).Name}s");

            // get by id
            group.MapGet("/{id}", async (
                    TId id, 
                    ICrudService<TAgg, TId> svc, 
                    CancellationToken ct) =>
            {
                var entity = await svc.GetAsync(id, ct);
                return entity is null ? Results.NotFound() : Results.Ok(entity);
            })
            .WithName($"Get{typeof(TAgg).Name}");

            // create
            group.MapPost("/", async (
                    [FromBody] object dto, 
                    ICrudService<TAgg, TId> svc, 
                    CancellationToken ct) =>
            {
                var created = await svc.CreateAsync(dto, ct);
                return Results.Ok(created);
            })
            .WithName($"Create{typeof(TAgg).Name}");

            // update
            group.MapPut("/{id}", async (
                    TId id,
                    [FromBody] object dto, 
                    ICrudService<TAgg, TId> svc,
                    CancellationToken ct) =>
            {
                var updated = await svc.UpdateAsync(id, dto, ct);
                return Results.Ok(updated);
            })
            .WithName($"Update{typeof(TAgg).Name}");

            // delete
            group.MapDelete("/{id}", async (
                    TId id, 
                    ICrudService<TAgg, TId> svc,
                    CancellationToken ct) =>
            {
                await svc.DeleteAsync(id, ct);
                return Results.NoContent();
            })
            .WithName($"Delete{typeof(TAgg).Name}");

            return builder;
        }
    }
}
