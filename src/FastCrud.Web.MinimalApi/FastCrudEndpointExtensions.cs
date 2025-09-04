using FastCrud.Abstractions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading;
using System.Threading.Tasks;

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
        /// <param name="builder">Endpoint route builder.</param>
        /// <param name="routePrefix">Route prefix (e.g., "/api/orders").</param>
        /// <returns>The route builder for chaining.</returns>
        public static IEndpointRouteBuilder MapFastCrud<TAgg, TId>(this IEndpointRouteBuilder builder, string routePrefix)
        {
            var group = builder.MapGroup(routePrefix);
            // list/query
            group.MapGet("/", async ([AsParameters] QuerySpec spec, ICrudService<TAgg, TId> svc, CancellationToken ct) =>
            {
                var result = await svc.QueryAsync(spec, ct);
                return Results.Ok(result);
            })
            .WithName($"List{typeof(TAgg).Name}s");

            // get by id
            group.MapGet("/{id}", async (TId id, ICrudService<TAgg, TId> svc, CancellationToken ct) =>
            {
                var entity = await svc.GetAsync(id, ct);
                return entity is null ? Results.NotFound() : Results.Ok(entity);
            })
            .WithName($"Get{typeof(TAgg).Name}");

            // create
            group.MapPost("/", async ([FromBody] object dto, ICrudService<TAgg, TId> svc, CancellationToken ct) =>
            {
                var created = await svc.CreateAsync(dto, ct);
                return Results.Ok(created);
            })
            .WithName($"Create{typeof(TAgg).Name}");

            // update
            group.MapPut("/{id}", async (TId id, [FromBody] object dto, ICrudService<TAgg, TId> svc, CancellationToken ct) =>
            {
                var updated = await svc.UpdateAsync(id, dto, ct);
                return Results.Ok(updated);
            })
            .WithName($"Update{typeof(TAgg).Name}");

            // delete
            group.MapDelete("/{id}", async (TId id, ICrudService<TAgg, TId> svc, CancellationToken ct) =>
            {
                await svc.DeleteAsync(id, ct);
                return Results.NoContent();
            })
            .WithName($"Delete{typeof(TAgg).Name}");

            return builder;
        }
    }
}
