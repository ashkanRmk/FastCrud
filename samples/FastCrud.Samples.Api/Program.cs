using FastCrud.Abstractions;
using FastCrud.Core.DI;
using FastCrud.Mapping.Mapster.DI;
using FastCrud.Query.Gridify.DI;
using FastCrud.Validation.FluentValidation.DI;
using FastCrud.Web.MinimalApi;
using Microsoft.AspNetCore.Builder;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddFastCrudCore(o =>
{
    o.SoftDeleteEnabled = true;
    o.AuditEnabled = false;
});

builder.Services.UseMapster();
builder.Services.UseGridifyQueryEngine();
builder.Services.UseFluentValidationAdapter();

var app = builder.Build();
app.MapFastCrud<object, Guid>("/api/sample"); // replace object with your aggregate type
app.Run();
