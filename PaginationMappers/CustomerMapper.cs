using FastCrud.Entities;
using Gridify;

namespace FastCrud.PaginationMappers;

public sealed class CustomerMapper : GridifyMapper<Customer>
{
    public CustomerMapper()
        : base(new GridifyMapperConfiguration
        {
            CaseInsensitiveFiltering = true
        })
    {
        AddMap(nameof(Customer.Id), c => c.Id);
        AddMap(nameof(Customer.FullName), c => c.FullName);
        AddMap(nameof(Customer.Email), c => c.Email);
        AddMap(nameof(Customer.CreatedAt), c => c.CreatedAt);
    }
}