using FastCrud.Samples.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace FastCrud.Samples.Api.Data;

public sealed class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Customer> Customers => Set<Customer>();
    public DbSet<Order> Orders => Set<Order>();
    public DbSet<AuditEntry> AuditEntries => Set<AuditEntry>();
}