namespace FastCrud.Core.Tests.Unit.Models;

public sealed class TestCustomer
{
    // private Customer()
    // {}

    public Guid Id { get; private set; }
    public string FirstName { get; private set; }
    public string LastName { get; private set; }
    public string Email { get; private set; }
    public DateTime CreatedUtc { get; private set; }
}