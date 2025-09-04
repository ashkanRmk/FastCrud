using FastCrud.Abstractions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace FastCrud.Web.MinimalApi;

public static class FastCrudEndpointExtensions
{
    public static IEndpointRouteBuilder MapFastCrud<TAgg, TId>(this IEndpointRouteBuilder app, string basePath = "/api/fastcrud")
    {
        var group = app.MapGroup(basePath);

        group.MapGet("/{id}", async (TId id, ICrudService<TAgg, TId> svc, CancellationToken ct)
            => await svc.GetAsync(id, ct));

        group.MapPost("/", async (TAgg dto, ICrudService<TAgg, TId> svc, CancellationToken ct)
            => await svc.CreateAsync(dto, ct));

        group.MapDelete("/{id}", async (TId id, ICrudService<TAgg, TId> svc, CancellationToken ct) =>
        {
            await svc.DeleteAsync(id, ct);
            return Results.NoContent();
        });

        // Extend with PUT/PATCH/Query endpoints as desired

        return app;
    }
}
