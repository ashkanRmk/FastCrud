using FastCrud.Samples.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace FastCrud.Samples.Api.Data;

public sealed class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Customer> Customers => Set<Customer>();
    public DbSet<Order> Orders => Set<Order>();
    public DbSet<AuditEntry> AuditEntries => Set<AuditEntry>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<AuditEntry>(entity =>
        {
            entity.Property(e => e.OldValues)
                .HasColumnType("nvarchar(max)");

            entity.Property(e => e.NewValues)
                .HasColumnType("nvarchar(max)");
        });
    }
}