namespace FastCrud.Samples.Api.Models;

public sealed class Customer
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public string FirstName { get; private set; } = default!;
    public string LastName { get; private set; } = default!;
    public string Email { get; private set; } = default!;
    public DateTime CreatedUtc { get; private set; } = DateTime.UtcNow;
}