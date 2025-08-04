using Crud.Generator.Data;
using Crud.Generator.Models;
using Microsoft.EntityFrameworkCore;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

// Example: Register CRUD endpoints for Product
RegisterCrudEndpoints<Product>(app, "/products");

app.Run();

void RegisterCrudEndpoints<TEntity>(WebApplication app, string route) where TEntity : class
{
    app.MapGet(route, async (IRepository<TEntity> repo) =>
        await repo.GetAllAsync());

    app.MapGet($"{route}/{{id:int}}", async (int id, IRepository<TEntity> repo) =>
        await repo.GetByIdAsync(id) is TEntity entity ? Results.Ok(entity) : Results.NotFound());

    app.MapPost(route, async (TEntity entity, IRepository<TEntity> repo) =>
        Results.Created($"{route}", await repo.AddAsync(entity)));

    app.MapPut($"{route}/{{id:int}}", async (int id, TEntity entity, IRepository<TEntity> repo) =>
        await repo.UpdateAsync(id, entity) is TEntity updated ? Results.Ok(updated) : Results.NotFound());

    app.MapDelete($"{route}/{{id:int}}", async (int id, IRepository<TEntity> repo) =>
        await repo.DeleteAsync(id) ? Results.NoContent() : Results.NotFound());
}