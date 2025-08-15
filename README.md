# 📁 FastCrud: Minimal API Generator

Minimal-API template in .NET 9 for auto-generating CRUD endpoints with DTOs, validation, pagination, filtering, and sorting—powered by **Gridify**, **FluentValidation**, and **Mapster**.

## Features

- **Auto-registered** CRUD with **Minimal API** and **DTOs**
- **Validation** via FluentValidation
- **Paging**, **filtering**, **sorting** using Gridify
- **Field allow-lists** via mappers to control exposed columns
- **Modular endpoints** per entity
- **Swagger/OpenAPI** documentation built-in

---

## 🧱 How to add a new entity with CRUD

Let’s add an example entity: `Order`

### 1. Define your Entity & DTOs

**`Entities/Order.cs`**

```csharp
public class Order : IEntity<int>
{
    public int Id { get; set; }
    public string CustomerName { get; set; } = default!;
    public decimal TotalAmount { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
```

**`Dtos/OrderDtos.cs`**

```csharp
public record OrderReadDto(int Id, string CustomerName, decimal TotalAmount, DateTime CreatedAt);
public record OrderCreateDto(string CustomerName, decimal TotalAmount);
public record OrderUpdateDto(string CustomerName, decimal TotalAmount);
```

---

### 2. Add Validators

**`PaginationMappers/OrderMapper.cs`**

```csharp
public class OrderCreateValidator : AbstractValidator<OrderCreateDto>
{
    public OrderCreateValidator()
    {
        RuleFor(x => x.CustomerName).NotEmpty();
        RuleFor(x => x.TotalAmount).GreaterThanOrEqualTo(0);
    }
}

public class OrderUpdateValidator : AbstractValidator<OrderUpdateDto>
{
    public OrderUpdateValidator()
    {
        RuleFor(x => x.CustomerName).NotEmpty();
        RuleFor(x => x.TotalAmount).GreaterThanOrEqualTo(0);
    }
}
```

---

### 3. Add a Gridify mapper (explicit allow-list)

**`PaginationMappers/OrderMapper.cs`**

```csharp
public sealed class OrderMapper : GridifyMapper<Order>
{
    public OrderMapper()
        : base(new GridifyMapperConfiguration
        {
            CaseInsensitiveFiltering = true
        })
    {
        AddMap(nameof(Order.Id),        o => o.Id);
        AddMap(nameof(Order.CustomerName), o => o.CustomerName);
        AddMap(nameof(Order.TotalAmount), o => o.TotalAmount);
        AddMap(nameof(Order.CreatedAt), o => o.CreatedAt);
    }
}
```

---

### 4. Create Endpoint Module

**`Endpoints/OrderEndpoints.cs`**

```csharp
public sealed class OrderEndpoints : CrudEndpointModule<Order, int, OrderReadDto, OrderCreateDto, OrderUpdateDto>
{
    public override string RoutePrefix => "/orders";
    public override CrudOps Ops => CrudOps.AllOps;

    protected override void MapCustomEndpoints(RouteGroupBuilder group)
    {
        // Optional: additional endpoint
        group.MapGet("/top/{count:int}", async (AppDbContext db, int count, CancellationToken ct) =>
        {
            var list = await db.Set<Order>()
                .OrderByDescending(o => o.TotalAmount)
                .Take(count)
                .Select(o => new OrderReadDto(o.Id, o.CustomerName, o.TotalAmount, o.CreatedAt))
                .ToListAsync(ct);
            return Results.Ok(list);
        });
    }
}
```

---

### 5. Run and Test

CRUD appears under `/v1/orders` in Swagger. It supports:

Paging (`?page=1&pageSize=10`)

Filtering (`?filter=CustomerName=*smith`)

Sorting (`?orderBy=TotalAmount desc`)

---

Visit/swagger UI at `https://localhost:5001/swagger`
