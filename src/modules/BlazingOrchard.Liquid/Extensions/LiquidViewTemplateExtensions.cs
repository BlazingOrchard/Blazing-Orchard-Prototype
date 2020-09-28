using System;
using System.IO;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using BlazingOrchard.Liquid.Services;
using Fluid;

namespace BlazingOrchard.Liquid.Extensions
{
    public static class LiquidViewTemplateExtensions
    {
        public static async Task<string> RenderAsync(this LiquidViewTemplate template, TextEncoder encoder, LiquidTemplateContext context, object model, Action<Scope> scopeAction)
        {
            try
            {
                await context.EnterScopeAsync(model, scopeAction);
                return await template.RenderAsync(context, encoder);
            }
            finally
            {
                context.ReleaseScope();
            }
        }

        public static async Task RenderAsync(this LiquidViewTemplate template, TextWriter writer, TextEncoder encoder, LiquidTemplateContext context, object model, Action<Scope> scopeAction)
        {
            try
            {
                await context.EnterScopeAsync(model, scopeAction);
                await template.RenderAsync(writer, encoder, context);
            }
            finally
            {
                context.ReleaseScope();
            }
        }
    }
}