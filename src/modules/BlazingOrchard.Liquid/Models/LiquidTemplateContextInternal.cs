using System;

namespace BlazingOrchard.Liquid.Models
{
    internal class LiquidTemplateContextInternal : LiquidTemplateContext
    {
        public const int MaxShapeRecursions = 3;

        public LiquidTemplateContextInternal(IServiceProvider services) : base(services) { }

        public bool IsInitialized { get; set; }

        public int ShapeRecursions { get; set; }
    }
}