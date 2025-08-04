using Crud.Generator.Data;
using Crud.Generator.Infrastructure;
using Crud.Generator.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Crud Minimal API Generator", Version = "v1" });
});

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

// Full CRUD
app.RegisterCrudEndpoints<Product>("/products");

// only GET & POST (skip Update, Delete, GetById)
app.RegisterCrudEndpoints<Customer>("/customers", CrudOps.GetAll | CrudOps.Create);

// exclude DELETE
// app.RegisterCrudEndpoints<Order>("/orders", CrudOps.All & ~CrudOps.Delete);

app.Run();
