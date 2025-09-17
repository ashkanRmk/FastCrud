using FastCrud.Query.Gridify.Abstractions;
using FastCrud.Samples.Api.Models;
using Gridify;

namespace FastCrud.Samples.Api.GridifyProfiles;

public sealed class CustomerGridifyProfile : IGridifyMapperProfile<Customer>
{
    public void Configure(GridifyMapper<Customer> m)
    {
        m.Configuration.CaseInsensitiveFiltering = true;
        m.GenerateMappings()
            .AddMap("name", c => c.FirstName + " " + c.LastName)
            .AddMap("tags.name", customer => customer.Tags.Select(t => t.Name))
            .RemoveMap(nameof(Customer.CreatedUtc));
    }
}