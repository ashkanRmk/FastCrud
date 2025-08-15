using FastCrud.Data;
using FastCrud.Dtos;
using FastCrud.Entities;
using FastCrud.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace FastCrud.Endpoints;

public sealed class ProductEndpoints()
    : CrudEndpointModule<Product, int, ProductReadDto, ProductCreateDto, ProductUpdateDto>
        (ops: CrudOps.AllOps)
{
    public override string RoutePrefix => "/products";

    protected override void MapCustomEndpoints(RouteGroupBuilder group)
    {
        group.MapGet("/top/{count:int}",
            async (AppDbContext db, int count, CancellationToken ct) =>
            {
                var items = await db.Products.AsNoTracking()
                    .OrderByDescending(p => p.Price)
                    .Take(count)
                    .Select(p => new ProductReadDto
                        { Id = p.Id, Name = p.Name, Price = p.Price, CreatedAt = p.CreatedAt })
                    .ToListAsync(ct);

                return Results.Ok(items);
            }
        ).WithSummary("Get top N products by price");
    }
}