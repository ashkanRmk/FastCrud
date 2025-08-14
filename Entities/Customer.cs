using Crud.Generator.Abstractions;

namespace Crud.Generator.Entities;

public class Customer : IEntity<Guid>
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string FullName { get; set; } = default!;
    public string Email { get; set; } = default!;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}