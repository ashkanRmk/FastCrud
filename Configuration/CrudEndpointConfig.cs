using Crud.Generator.Entities;
using Crud.Generator.Infrastructure;

namespace Crud.Generator.Configuration;

public static class CrudEndpointConfig
{
    public static void RegisterV1CrudRoutes(this WebApplication app)
    {
        // Full CRUD
        app.RegisterCrudEndpoints<Product>();

        // Only GET (all) and POST
        app.RegisterCrudEndpoints<Customer>(CrudOps.GetAll | CrudOps.Create);
    }

    public static void RegisterV2CrudRoutes(this WebApplication app)
    {
        // All except DELETE
        app.RegisterCrudEndpoints<Customer>(CrudOps.All & ~CrudOps.Delete, "v2");
    }
}