using FastCrud.Abstractions.Query;
using FastCrud.Core;
using Gridify;
using Microsoft.Extensions.DependencyInjection;

namespace FastCrud.Query.Gridify.DI
{
    public static class GridifyServiceCollectionExtensions
    {
        public static IServiceCollection UseSimpleQueryEngine(this IServiceCollection services)
        {
            services.AddSingleton<IQueryEngine, SimpleQueryEngine>();
            return services;
        }
        
        public static IServiceCollection UseGridifyQueryEngine(
            this IServiceCollection services,
            IGridifyMapperProvider? mapperProvider = null)
        {
            services.AddSingleton<IQueryEngine, GridifyQueryEngine>();
            services.AddSingleton<IGridifyMapperProvider>(sp => mapperProvider ?? new DefaultGridifyMapperProvider());
            return services;
        }
        
        /// <summary>
        /// Convenience overload to register a single global GridifyMapper for a T.
        /// For more control, implement IGridifyMapperProvider yourself.
        /// </summary>
        public static IServiceCollection AddGridifyMapperFor<T>(
            this IServiceCollection services,
            IGridifyMapper<T> mapper)
        {
            services.AddSingleton<IGridifyMapperProvider>(sp => new SingleMapperProvider<T>(mapper));
            return services;
        }
        
        private sealed class SingleMapperProvider<T>(IGridifyMapper<T> mapper) : IGridifyMapperProvider
        {
            public IGridifyMapper<TU>? GetMapper<TU>()
                => typeof(TU) == typeof(T) ? (IGridifyMapper<TU>)(object)mapper : null;
        }
    }
}
