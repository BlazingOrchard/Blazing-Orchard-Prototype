using BlazingOrchard.DisplayManagement.Services;
using Microsoft.Extensions.DependencyInjection;

namespace BlazingOrchard.DisplayManagement.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddDisplayManagement(this IServiceCollection services)
        {
            return services
                .AddSingleton<IComponentTypeProvider, ComponentTypeProvider>()
                .AddSingleton<IShapeFactory, ShapeFactory>()
                .AddSingleton<IShapeTableManager, ShapeTableManager>()
                .AddSingleton<IShapeRenderer, ShapeRenderer>()
                .AddSingleton<IComponentRenderer, ComponentRenderer>();
        }
    }
}