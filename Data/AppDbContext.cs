using Crud.Generator.Models;
using Microsoft.EntityFrameworkCore;

namespace Crud.Generator.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Product> Products => Set<Product>();

    // Add new DbSets here for new entities.
}