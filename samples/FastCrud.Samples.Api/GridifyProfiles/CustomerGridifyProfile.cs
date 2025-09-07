using FastCrud.Query.Gridify;
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
            .RemoveMap(nameof(Customer.CreatedUtc));
    }
}