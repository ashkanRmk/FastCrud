using FastCrud.Abstractions;

namespace FastCrud.Entities;

public class Product : IEntity<int>
{
    public int Id { get; set; }
    public string Name { get; set; } = default!;
    public decimal Price { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}