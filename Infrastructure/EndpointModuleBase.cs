using SharpGrip.FluentValidation.AutoValidation.Endpoints.Extensions;

namespace FastCrud.Infrastructure;

public abstract class EndpointModuleBase : IEndpointModule
{
    public abstract string RoutePrefix { get; }
    public virtual string[] Tags => [RoutePrefix.Trim('/').ToUpperInvariant()];

    public void Register(RouteGroupBuilder apiVGroup)
    {
        var group = apiVGroup.MapGroup(RoutePrefix)
            .WithOpenApi()
            .WithTags(Tags)
            .AddFluentValidationAutoValidation();
        
        MapEndpoints(group);
    }

    protected abstract void MapEndpoints(RouteGroupBuilder group);
}