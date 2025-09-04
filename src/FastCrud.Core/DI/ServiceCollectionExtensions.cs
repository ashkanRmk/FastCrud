using FastCrud.Abstractions;
using FastCrud.Abstractions.Options;
using FastCrud.Core.Infrastructure;
using FastCrud.Core.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace FastCrud.Core.DI;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddFastCrudCore(this IServiceCollection services, Action<FastCrudOptions>? configure = null)
    {
        if (configure != null) services.Configure(configure);
        services.AddSingleton<IFastCrudClock, SystemClock>();
        services.AddScoped(typeof(ICrudService<,>), typeof(CrudService<,>));
        return services;
    }

    public static FastCrudOptions GetFastCrudOptions(this IServiceProvider sp)
        => sp.GetRequiredService<IOptions<FastCrudOptions>>().Value;
}
