using Crud.Generator.Entities;
using Microsoft.EntityFrameworkCore;

namespace Crud.Generator.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Product> Products => Set<Product>();
    public DbSet<Customer> Customers => Set<Customer>();
}