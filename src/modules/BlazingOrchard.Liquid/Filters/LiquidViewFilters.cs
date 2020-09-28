using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using BlazingOrchard.DisplayManagement.Extensions;
using BlazingOrchard.DisplayManagement.Models;
using BlazingOrchard.DisplayManagement.Services;
using BlazingOrchard.DisplayManagement.Shapes;
using Fluid;
using Fluid.Values;
using Humanizer;

namespace BlazingOrchard.Liquid.Filters
{
    public static class LiquidViewFilters
    {
        private static readonly AsyncFilterDelegate _htmlClassDelegate = HtmlClass;
        private static readonly AsyncFilterDelegate _newShapeDelegate = NewShape;
        private static readonly AsyncFilterDelegate _shapeRenderDelegate = ShapeRender;
        private static readonly AsyncFilterDelegate _shapeStringifyDelegate = ShapeStringify;
        private static readonly FilterDelegate _shapePropertiesDelegate = ShapeProperties;

        public static FilterCollection WithLiquidViewFilters(this FilterCollection filters)
        {
            filters.AddAsyncFilter("html_class", _htmlClassDelegate);
            filters.AddAsyncFilter("shape_new", _newShapeDelegate);
            filters.AddAsyncFilter("shape_render", _shapeRenderDelegate);
            filters.AddAsyncFilter("shape_stringify", _shapeStringifyDelegate);
            filters.AddFilter("shape_properties", _shapePropertiesDelegate);

            return filters;
        }

        public static ValueTask<FluidValue> HtmlClass(FluidValue input, FilterArguments arguments, TemplateContext context)
        {
            return new ValueTask<FluidValue>(new StringValue(input.ToStringValue().Camelize()));
        }

        public static ValueTask<FluidValue> NewShape(FluidValue input, FilterArguments arguments, TemplateContext context)
        {
            static async ValueTask<FluidValue> Awaited(ValueTask<IShape> task)
            {
                return FluidValue.Create(await task);
            }

            if (!context.AmbientValues.TryGetValue("ShapeFactory", out var item) || !(item is IShapeFactory shapeFactory))
            {
                return ThrowArgumentException<ValueTask<FluidValue>>("ShapeFactory missing while invoking 'shape_new'");
            }

            var type = input.ToStringValue();
            var properties = new Dictionary<string, object>(arguments.Count);

            foreach (var name in arguments.Names)
            {
                properties.Add(name.Pascalize().Underscore(), arguments[name].ToObjectValue());
            }

            var task = shapeFactory.CreateAsync(type, Arguments.From(properties));
            if (!task.IsCompletedSuccessfully)
            {
                return Awaited(task);
            }
            return new ValueTask<FluidValue>(FluidValue.Create(task.Result));
        }

        public static ValueTask<FluidValue> ShapeStringify(FluidValue input, FilterArguments arguments, TemplateContext context)
        {
            static async ValueTask<FluidValue> Awaited(Task<string> task) => new StringValue(await task);

            if (input.ToObjectValue() is IShape shape)
            {
                if (!context.AmbientValues.TryGetValue("DisplayHelper", out var item) || !(item is IShapeRenderer displayHelper))
                    return ThrowArgumentException<ValueTask<FluidValue>>(
                        "DisplayHelper missing while invoking 'shape_stringify'");

                var task = displayHelper.RenderShapeAsStringAsync(shape);
                return !task.IsCompletedSuccessfully ? Awaited(task) : new ValueTask<FluidValue>(new StringValue(task.Result));
            }

            return new ValueTask<FluidValue>(NilValue.Instance);
        }

        public static ValueTask<FluidValue> ShapeRender(FluidValue input, FilterArguments arguments, TemplateContext context)
        {
            static async ValueTask<FluidValue> Awaited(Task<string> task) => new StringValue(await task);

            if (input.ToObjectValue() is IShape shape)
            {
                if (!context.AmbientValues.TryGetValue("DisplayHelper", out var item) || !(item is IShapeRenderer displayHelper))
                    return ThrowArgumentException<ValueTask<FluidValue>>(
                        "DisplayHelper missing while invoking 'shape_render'");

                var task = displayHelper.RenderShapeAsStringAsync(shape);
                return !task.IsCompletedSuccessfully ? Awaited(task) : new ValueTask<FluidValue>(new StringValue(task.Result));
            }

            return new ValueTask<FluidValue>(NilValue.Instance);
        }

        public static FluidValue ShapeProperties(FluidValue input, FilterArguments arguments, TemplateContext context)
        {
            if (input.ToObjectValue() is IShape shape)
            {
                foreach (var name in arguments.Names)
                    shape.Properties[name.Pascalize().Underscore()] = arguments[name].ToObjectValue();
                
                return FluidValue.Create(shape);
            }

            return NilValue.Instance;
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private static T ThrowArgumentException<T>(string message) => throw new ArgumentException(message);
    }
}
