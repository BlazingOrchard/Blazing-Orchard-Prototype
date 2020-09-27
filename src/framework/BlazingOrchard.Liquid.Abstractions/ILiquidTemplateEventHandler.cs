using System.Threading.Tasks;
using Fluid;

namespace BlazingOrchard.Liquid
{
    public interface ILiquidTemplateEventHandler
    {
        Task RenderingAsync(TemplateContext context);
    }
}
