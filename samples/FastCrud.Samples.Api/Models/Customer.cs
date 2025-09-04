namespace FastCrud.Samples.Api.Models;

/// <summary>
/// Represents a customer in the FastCrud sample application.
/// </summary>
public sealed class Customer
{
    /// <summary>
    /// Gets or sets the unique identifier for the customer.
    /// </summary>
    public Guid Id { get; set; } = Guid.NewGuid();

    /// <summary>
    /// Gets or sets the first name of the customer.
    /// </summary>
    public string FirstName { get; set; } = default!;

    /// <summary>
    /// Gets or sets the last name of the customer.
    /// </summary>
    public string LastName { get; set; } = default!;

    /// <summary>
    /// Gets or sets the email address of the customer.
    /// </summary>
    public string Email { get; set; } = default!;

    /// <summary>
    /// Gets or sets the UTC timestamp when the customer was created.
    /// </summary>
    public DateTime CreatedUtc { get; set; } = DateTime.UtcNow;
}