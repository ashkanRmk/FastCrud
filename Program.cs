using FastCrud.Data;
using FastCrud.Dtos;
using FastCrud.Entities;
using FastCrud.Infrastructure;
using FastCrud.Repositories;
using FastCrud.Validations;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using SharpGrip.FluentValidation.AutoValidation.Endpoints.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(opt =>
    opt.UseNpgsql(builder.Configuration.GetConnectionString("Default")));

builder.Services.AddScoped(typeof(IGenericRepository<,>), typeof(GenericRepository<,>));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddValidatorsFromAssemblyContaining<ProductCreateValidator>();
builder.Services.AddFluentValidationAutoValidation();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await db.Database.EnsureCreatedAsync();
}

app.UseSwagger();
app.UseSwaggerUI();

var v1 = app.MapGroup("/v1").AddFluentValidationAutoValidation();

app.MapCrudEndpoints<Product, int, ProductReadDto, ProductCreateDto, ProductUpdateDto>("/products", CrudOps.All);
app.MapCrudEndpoints<Customer, Guid, CustomerReadDto, CustomerCreateDto, CustomerUpdateDto>("/customers", CrudOps.All & ~CrudOps.Delete);

app.Run();