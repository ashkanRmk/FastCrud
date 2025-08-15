using FastCrud.Dtos;
using FastCrud.Entities;
using FastCrud.Infrastructure;
using FastCrud.Repositories;
using Microsoft.EntityFrameworkCore;

namespace FastCrud.Endpoints;

public sealed class CustomerEndpoints()
    : CrudEndpointModule<Customer, Guid, CustomerReadDto, CustomerCreateDto, CustomerUpdateDto>
{
    public override string RoutePrefix => "/customers";
    public override CrudOps Ops => CrudOps.AllOps & ~CrudOps.Delete;

    protected override void MapCustomEndpoints(RouteGroupBuilder group)
    {
        group.MapGet("/by-domain/{domain}",
            async (IGenericRepository<Customer, Guid> repo, string domain, CancellationToken ct) =>
            {
                var list = await repo.Query()
                    .Where(c => c.Email.EndsWith("@" + domain))
                    .Select(c => new CustomerReadDto
                        { Id = c.Id, FullName = c.FullName, Email = c.Email, CreatedAt = c.CreatedAt })
                    .ToListAsync(ct);

                return Results.Ok(list);
            }
        ).WithSummary("Find customers by email domain");
    }
}