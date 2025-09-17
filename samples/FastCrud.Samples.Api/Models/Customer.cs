namespace FastCrud.Samples.Api.Models;

public sealed class Customer
{
    private Customer()
    {
    }
    public Customer(
        Guid id,
        string firstName,
        string lastName,
        string email,
        DateTime createdUtc,
        List<Tag> tags
        )
    {
        Id = id;
        FirstName = firstName;
        LastName = lastName;
        Email = email;
        CreatedUtc = createdUtc;
        Tags = tags;
    }
    public Guid Id { get; private set; }
    public string FirstName { get; private set; }
    public string LastName { get; private set; }
    public string Email { get; private set; }
    public DateTime CreatedUtc { get; private set; }

    public List<Tag> Tags { get; set; } = [];
}