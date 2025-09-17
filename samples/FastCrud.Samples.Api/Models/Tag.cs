namespace FastCrud.Samples.Api.Models;

public class Tag
{
    public Guid Id { get; private set; }
    public string Name { get; set; }
    public string Description { get; set; }

    public List<Customer> Customers { get; set; } = [];
}