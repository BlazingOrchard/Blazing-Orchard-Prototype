using System;
using System.Collections.Generic;

namespace BlazingOrchard.Liquid
{
    public class LiquidOptions
    {
        public Dictionary<string, Type> FilterRegistrations { get; } = new Dictionary<string, Type>();
    }
}
