using FastCrud.Samples.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace FastCrud.Samples.Api.Data;

/// <summary>
/// Entity Framework Core DbContext for the FastCrud sample. This context uses
/// the in-memory database provider for demonstration purposes.
/// </summary>
public sealed class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    /// <summary>
    /// Gets the set of customers.
    /// </summary>
    public DbSet<Customer> Customers => Set<Customer>();

    /// <summary>
    /// Gets the set of orders.
    /// </summary>
    public DbSet<Order> Orders => Set<Order>();
}