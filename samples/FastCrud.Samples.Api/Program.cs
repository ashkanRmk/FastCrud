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

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseInMemoryDatabase("fastcrud-demo"));

// FastCrud core services
builder.Services.AddFastCrudCore();
builder.Services.UseMapster();
builder.Services.UseGridifyQueryEngine();
builder.Services.UseFluentValidationAdapter();

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

// Map CRUD endpoints for entities using FastCrud.
app.MapFastCrud<Customer, Guid, CustomerCreateDto, CustomerUpdateDto, CustomerReadDto>("/api/customers");
app.MapFastCrud<Order, Guid, OrderCreateDto, OrderUpdateDto, OrderReadDto>("/api/orders", ~CrudOps.Delete);

app.Run();