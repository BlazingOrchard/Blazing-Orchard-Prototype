using System;
using System.Globalization;
using System.Threading.Tasks;
using BlazingOrchard.DisplayManagement.Services;
using BlazingOrchard.DisplayManagement.Shapes;
using BlazingOrchard.Liquid.Models;
using BlazingOrchard.Liquid.Services;
using Fluid;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace BlazingOrchard.Liquid.Extensions
{
    public static class LiquidTemplateContextExtensions
    {
        internal static async Task EnterScopeAsync(this LiquidTemplateContext context, object? model, Action<Scope> scopeAction)
        {
            if (context is LiquidTemplateContextInternal contextInternal)
            {
                if (!contextInternal.IsInitialized)
                {
                    context.AmbientValues.EnsureCapacity(9);
                    context.AmbientValues.Add("Services", context.Services);

                    var shapeRenderer = context.Services.GetRequiredService<IShapeRenderer>();
                    context.AmbientValues.Add("ShapeRenderer", shapeRenderer);

                    var shapeFactory = context.Services.GetRequiredService<IShapeFactory>();
                    context.AmbientValues.Add("ShapeFactory", shapeFactory);

                    var options = context.Services.GetRequiredService<IOptions<LiquidOptions>>().Value;

                    context.AddAsyncFilters(options);

                    foreach (var handler in context.Services.GetServices<ILiquidTemplateEventHandler>())
                        await handler.RenderingAsync(context);

                    context.CultureInfo = CultureInfo.CurrentUICulture;
                    contextInternal.IsInitialized = true;
                }

                context.EnterChildScope();

                if (model != null) 
                    context.MemberAccessStrategy.Register(model.GetType());

                if (context.GetValue("Model")?.ToObjectValue() == model && model is IShape shape)
                {
                    if (contextInternal.ShapeRecursions++ > LiquidTemplateContextInternal.MaxShapeRecursions)
                        throw new InvalidOperationException(
                            $"The '{shape.Metadata.Type}' shape has been called recursively more than {LiquidTemplateContextInternal.MaxShapeRecursions} times.");
                }
                else
                {
                    contextInternal.ShapeRecursions = 0;
                }
            }

            context.SetValue("Model", model);
            scopeAction?.Invoke(context.LocalScope);
        }

        internal static void AddAsyncFilters(this LiquidTemplateContext context, LiquidOptions options)
        {
            context.Filters.EnsureCapacity(options.FilterRegistrations.Count);

            foreach (var registration in options.FilterRegistrations)
            {
                context.Filters.AddAsyncFilter(registration.Key, (input, arguments, ctx) =>
                {
                    var filter = (ILiquidFilter)context.Services.GetRequiredService(registration.Value);
                    return filter.ProcessAsync(input, arguments, ctx);
                });
            }
        }
    }
}