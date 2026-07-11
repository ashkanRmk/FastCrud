namespace FastCrud.Samples.Api.Models;

public class Tag
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = default!;
    public string Description { get; set; } = default!;

    public List<Customer> Customers { get; set; } = [];
}
