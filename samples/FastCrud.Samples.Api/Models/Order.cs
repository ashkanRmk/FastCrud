namespace FastCrud.Samples.Api.Models;

/// <summary>
/// Represents an order placed by a customer in the FastCrud sample application.
/// </summary>
public sealed class Order
{
    /// <summary>
    /// Gets or sets the unique identifier for the order.
    /// </summary>
    public Guid Id { get; set; } = Guid.NewGuid();

    /// <summary>
    /// Gets or sets the identifier of the customer who placed the order.
    /// </summary>
    public Guid CustomerId { get; set; }

    /// <summary>
    /// Gets or sets the order number (e.g., ORD-2025-0001).
    /// </summary>
    public string Number { get; set; } = default!;

    /// <summary>
    /// Gets or sets the monetary amount of the order.
    /// </summary>
    public decimal Amount { get; set; }

    /// <summary>
    /// Gets or sets the UTC timestamp when the order was placed.
    /// </summary>
    public DateTime PlacedUtc { get; set; } = DateTime.UtcNow;

    // Navigation property could be added for EF relationship if needed
    // public Customer? Customer { get; set; }
}