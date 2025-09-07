# 🚀 FastCrud — Minimal API CRUD Generator for .NET 9

**FastCrud** is a lightweight, flexible **CRUD API generator** for **.NET 9**.  
It combines **Minimal APIs + EF Core + DTOs + FluentValidation + Gridify + Auto-Swagger** to let you bootstrap production-ready endpoints with minimal effort.

---

## 📦 Install NuGet packages

Add the required FastCrud packages (version `0.2.1`):

```bash
dotnet add package FastCrud.Abstractions -v 0.2.1
dotnet add package FastCrud.Core -v 0.2.1
dotnet add package FastCrud.Mapping.Mapster -v 0.2.1
dotnet add package FastCrud.PersistenceEfCore -v 0.2.1
dotnet add package FastCrud.Query.Gridify -v 0.2.1
dotnet add package FastCrud.Validation.FluentValidation -v 0.2.1
dotnet add package FastCrud.Web.MinimalApi -v 0.2.1
```
---

## ⚙️ Configure Program.cs

Register FastCrud services:
```csharp
builder.Services.AddFastCrudCore();
builder.Services.UseMapster();
builder.Services.UseGridifyQueryEngine();
builder.Services.UseFluentValidationAdapter();
```
Register EF repositories per entity:
```csharp
builder.Services.AddEfRepository<Customer, Guid, AppDbContext>();
builder.Services.AddEfRepository<Order, Guid, AppDbContext>();
```
---

## 🌐 Map CRUD Endpoints
Define CRUD endpoints for each entity + DTO set:

```csharp
app.MapFastCrud<Customer, Guid, CustomerCreateDto, CustomerUpdateDto, CustomerReadDto>(
    "/api/customers",
    nameof(Customer),
    "v1"
);

app.MapFastCrud<Order, Guid, OrderCreateDto, OrderUpdateDto, OrderReadDto>(
    "/api/orders",
    nameof(Order),
    "v1",
    ~CrudOps.Delete // disable Delete
);
```

- Each entity requires 3 DTOs: CreateDto, UpdateDto, and ReadDto.
- By default, all CRUD operations are generated.
- To restrict operations, use the CrudOps flag with bitwise operators.

---

## ✅ Validation

Add FluentValidation validators for your DTOs or entities:
```csharp
public class CustomerValidator : AbstractValidator<Customer>
{
    public CustomerValidator()
    {
        RuleFor(x => x.FirstName).NotEmpty().WithMessage("First name is required.");
        RuleFor(x => x.LastName).NotEmpty();
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
    }
}
```
Validation is applied automatically to requests.

----
## 🔍 Filtering & Sorting with Gridify

Enable advanced query options by writing a `Gridify` profile:
```csharp
public sealed class CustomerGridifyProfile : IGridifyMapperProfile<Customer>
{
    public void Configure(GridifyMapper<Customer> m)
    {
        m.Configuration.CaseInsensitiveFiltering = true;

        m.GenerateMappings()
         .AddMap("name", c => c.FirstName + " " + c.LastName)
         .RemoveMap(nameof(Customer.CreatedUtc));
    }
}
```
This enables expressive queries like:
- `GET /api/customers?filter=name~"john"&sort=-CreatedUtc&page=1&pageSize=20`

---
## 🎉 Done!

With just a few lines, you get:

- Clean Minimal APIs
- Automatic Swagger docs
- DTO mapping (Mapster)
- Validation (FluentValidation)
- Filtering, sorting & paging (Gridify)
