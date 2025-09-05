using FastCrud.Abstractions.Abstractions;
using FastCrud.Core.DI;
using FastCrud.Mapping.Mapster.DI;
using FastCrud.Persistence.EFCore.DI;
using FastCrud.Query.Gridify.DI;
using FastCrud.Samples.Api.Data;
using FastCrud.Samples.Api.Dtos;
using FastCrud.Samples.Api.Models;
using FastCrud.Validation.FluentValidation.DI;
using FastCrud.Web.MinimalApi;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// In-memory database; replace with SQL Server or other provider in real apps.
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseInMemoryDatabase("fastcrud-demo"));

// FastCrud core services
builder.Services.AddFastCrudCore();

// Mapster configuration: map DTOs onto entities.
builder.Services.UseMapster(cfg =>
{
    cfg.NewConfig<CustomerCreateDto, Customer>();
    cfg.NewConfig<CustomerUpdateDto, Customer>();
    cfg.NewConfig<OrderCreateDto, Order>();
    cfg.NewConfig<OrderUpdateDto, Order>();
});

// Query engine: register a Gridify-based engine. Currently uses SimpleQueryEngine under the hood.
builder.Services.UseGridifyQueryEngine();

// FluentValidation: scan validators in this assembly and bridge to IModelValidator<T>.
builder.Services.UseFluentValidationAdapter(typeof(Program).Assembly);

// Register EF repositories per aggregate. Required for CrudService to resolve IRepository.
builder.Services.AddEfRepository<Customer, Guid, AppDbContext>();
builder.Services.AddEfRepository<Order, Guid, AppDbContext>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "FastCrud Sample API",
        Version = "v1",
        Description = "Demo API showcasing FastCrud with Customer and Order entities using EF Core InMemory"
    });
});

var app = builder.Build();

// Seed some sample data when the app starts
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    if (!db.Customers.Any())
    {
        var c1 = new Customer { FirstName = "Samane", LastName = "Yaghoubi", Email = "samane@example.com" };
        var c2 = new Customer { FirstName = "Ashkan", LastName = "Rahmani", Email = "ashkan@example.com" };
        db.Customers.AddRange(c1, c2);

        db.Orders.AddRange(
            new Order { CustomerId = c1.Id, Number = "ORD-2025-0001", Amount = 120.50m },
            new Order { CustomerId = c2.Id, Number = "ORD-2025-0002", Amount = 260.00m }
        );
        db.SaveChanges();
    }
}

app.UseSwagger();
app.UseSwaggerUI();

// Map CRUD endpoints for entities using FastCrud. The route prefixes must begin with '/'.
app.MapFastCrud<Customer, Guid>("/api/customers");
app.MapFastCrud<Order, Guid>("/api/orders");

// Additional DTO-specific endpoints to demonstrate mapping from DTOs into entities via FastCrud services.
app.MapPost("/api/customers-dto", async (CustomerCreateDto dto, ICrudService<Customer, Guid> svc, CancellationToken ct) =>
{
    var entity = await svc.CreateAsync(dto, ct);
    return Results.Ok(entity);
}).WithName("CreateCustomerDto").WithTags("Customers");

app.MapPut("/api/customers-dto/{id:guid}", async (Guid id, CustomerUpdateDto dto, ICrudService<Customer, Guid> svc, CancellationToken ct) =>
{
    var entity = await svc.UpdateAsync(id, dto, ct);
    return Results.Ok(entity);
}).WithName("UpdateCustomerDto").WithTags("Customers");

app.MapPost("/api/orders-dto", async (OrderCreateDto dto, ICrudService<Order, Guid> svc, CancellationToken ct) =>
{
    var entity = await svc.CreateAsync(dto, ct);
    return Results.Ok(entity);
}).WithName("CreateOrderDto").WithTags("Orders");

app.MapPut("/api/orders-dto/{id:guid}", async (Guid id, OrderUpdateDto dto, ICrudService<Order, Guid> svc, CancellationToken ct) =>
{
    var entity = await svc.UpdateAsync(id, dto, ct);
    return Results.Ok(entity);
}).WithName("UpdateOrderDto").WithTags("Orders");

app.Run();