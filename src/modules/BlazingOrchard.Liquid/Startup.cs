using BlazingOrchard.Contents;
using BlazingOrchard.Contents.Display.Extensions;
using BlazingOrchard.Liquid.Drivers;
using BlazingOrchard.Liquid.Models;
using BlazingOrchard.Liquid.Services;
using BlazingOrchard.Modules;
using Microsoft.Extensions.DependencyInjection;

namespace BlazingOrchard.Liquid
{
    public class Startup : IStartup
    {
        public void AddServices(IServiceCollection services)
        {
            services
                .AddContentPart<LiquidPart>()
                .AddContentPartDisplayDriver<LiquidPartDisplay>()
                .AddSingleton<ILiquidTemplateManager, LiquidTemplateManager>();
        }
    }
}