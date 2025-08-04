# 📁 Crud Minimal API Generator

A .NET 9+, PostgreSQL-based Minimal API starter that provides easy CRUD:

* Generic repository pattern with EF Core
* Generic `CrudRegistrar.RegisterCrudEndpoints<TEntity>()` logic
* Optional inclusion/exclusion of specific HTTP methods via flags
* Swagger/OpenAPI support via `.WithOpenApi()` and `MapOpenApi()`

---

## ✨ Why this setup?

* **Minimal code overhead** — declare your entity once, get all CRUD for free
* **Per-entity customization** — skip or restrict operations like DELETE
* **Self-documenting** — `.WithOpenApi()` generates clean Swagger UI automatically
* **Extensible** — easy to override individual endpoints (example included)

This readme shows how to add **new CRUD steps** and customize per-entity options.

---

## 🧱 How to add a new entity with CRUD

Let’s add an example entity: `Customer`

### 1. Create the model

**`Models/Customer.cs`**

```csharp
namespace CrudMinimalApi.Models;

public class Customer
{
    public int Id { get; set; }
    public string FirstName { get; set; } = "";
    public string LastName  { get; set; } = "";
    public string Email     { get; set; } = "";
}
```

---

### 2. Register the DbSet in your DbContext

**`Data/AppDbContext.cs`**

```csharp
public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Product> Products => Set<Product>();
    public DbSet<Customer> Customers => Set<Customer>();
}
```

---

### 3. Add CRUD endpoint registration in `Program.cs`

Import the registrar:

```csharp
using CrudMinimalApi.Infrastructure;
```

Inside `app` build section, pick one of the following:

| Operation               | Code snippet                                                                                                |
| ----------------------- |-------------------------------------------------------------------------------------------------------------|
| Full CRUD (default)     | `app.RegisterCrudEndpoints<Customer>("/customers");`                                                        |
| Only GET & POST         | `app.RegisterCrudEndpoints<Customer>("/customers", CrudOps.GetAll \| CrudOps.Create);\`                     |
| Skip DELETE             | `app.RegisterCrudEndpoints<Customer>("/customers", CrudOps.All & ~CrudOps.Delete);`                         |
| No endpoints (override) | `app.RegisterCrudEndpoints<Customer>("/customers", CrudOps.None);`                                          |

---

### 4. Apply migrations and run

```bash
dotnet ef migrations add AddCustomer
dotnet ef database update
dotnet run
```

Visit/swagger UI at `https://localhost:5001/swagger`
