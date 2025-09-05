namespace FastCrud.Samples.Api.Models;

public sealed class Customer
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string FirstName { get; set; } = default!;
    public string LastName { get; set; } = default!;
    public string Email { get; set; } = default!;
    public DateTime CreatedUtc { get; set; } = DateTime.UtcNow;
}