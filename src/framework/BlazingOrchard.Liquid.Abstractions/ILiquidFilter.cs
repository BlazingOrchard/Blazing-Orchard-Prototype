using System.Threading.Tasks;
using Fluid;
using Fluid.Values;

namespace BlazingOrchard.Liquid
{
    public interface ILiquidFilter
    {
        ValueTask<FluidValue> ProcessAsync(FluidValue input, FilterArguments arguments, TemplateContext ctx);
    }
}
