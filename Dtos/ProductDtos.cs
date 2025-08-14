namespace Crud.Generator.Dtos;

public sealed class ProductReadDto
{
    public int Id { get; set; }
    public string Name { get; set; } = default!;
    public decimal Price { get; set; }
    public DateTime CreatedAt { get; set; }
}

public sealed class ProductCreateDto
{
    public string Name { get; set; } = default!;
    public decimal Price { get; set; }
}

public sealed class ProductUpdateDto
{
    public string Name { get; set; } = default!;
    public decimal Price { get; set; }
}