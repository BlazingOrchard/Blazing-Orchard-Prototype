using System;
using BlazingOrchard.DisplayManagement.Shapes;
using Fluid.Accessors;

namespace BlazingOrchard.Liquid.Accessors
{
    internal class ShapeAccessor : DelegateAccessor
    {
        public ShapeAccessor() : base(Getter)
        {
        }

        private static Func<object, string, object?> Getter => (o, n) =>
        {
            if (!(o is Shape shape)) 
                return null;
            
            if (shape.Properties.TryGetValue(n, out var result))
                return result;

            if (n == "Items")
                return shape.Items;

            // Resolves Model.Content.MyType-MyField-FieldType_Display__DisplayMode
            var namedShaped = shape.Named(n);
            
            if (namedShaped != null)
                return namedShaped;

            // Resolves Model.Content.MyNamedPart
            // Resolves Model.Content.MyType__MyField
            // Resolves Model.Content.MyType-MyField
            return shape.NormalizedNamed(n.Replace("__", "-"));
        };
    }
}