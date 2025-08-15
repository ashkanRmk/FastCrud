using FastCrud.Entities;
using Gridify;

namespace FastCrud.PaginationMappers;

public sealed class ProductMapper : GridifyMapper<Product>
{
    public ProductMapper()
        : base(new GridifyMapperConfiguration
        {
            CaseInsensitiveFiltering = true
        })
    {
        AddMap(nameof(Product.Id), p => p.Id);
        AddMap(nameof(Product.Name), p => p.Name);
        AddMap(nameof(Product.Price), p => p.Price);
        AddMap(nameof(Product.CreatedAt), p => p.CreatedAt);
    }
}