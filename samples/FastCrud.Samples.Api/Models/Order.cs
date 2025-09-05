namespace FastCrud.Samples.Api.Models;

public sealed class Order
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid CustomerId { get; set; }
    public string Number { get; set; } = default!;
    public decimal Amount { get; set; }
    public DateTime PlacedUtc { get; set; } = DateTime.UtcNow;
}