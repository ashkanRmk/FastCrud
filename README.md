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

---

## 🚩 Understanding the `CrudOps` flags

These flags determine which HTTP methods are wired for CRUD:

```csharp
[Flags]
public enum CrudOps
{
  None    = 0,
  GetAll  = 1 << 0,   // 00001
  GetById = 1 << 1,   // 00010
  Create  = 1 << 2,   // 00100
  Update  = 1 << 3,   // 01000
  Delete  = 1 << 4,   // 10000
  All     = GetAll | GetById | Create | Update | Delete
}
```

* The `[Flags]` attribute indicates this enum can be used as a **bit set**: combine values with OR (`|`), test with `HasFlag`, invert with `~`, etc. ([Medium][1], [Medium][2], [Microsoft Learn][3], [Stack Overflow][4], [Martin Costello's Blog][5], [Stack Overflow][6])
* `1 << n` is a C# bit-shift operator that yields `2ⁿ` without manual power-of-two calculations. ([Microsoft Learn][7])
* For example, `CrudOps.All & ~CrudOps.Delete` includes all ops except DELETE.

---

## 🧬 Source of truth: the `CrudRegistrar`

**`Infrastructure/CrudRegistrar.cs`**

* Contains `RegisterCrudEndpoints<TEntity>()` extension
* Registers routes only if the respective `CrudOps` flag is set
* Default `CrudOps.All` ensures no behavior change if flags are omitted
* Automatically applies `.WithOpenApi()` for Swagger documentation

