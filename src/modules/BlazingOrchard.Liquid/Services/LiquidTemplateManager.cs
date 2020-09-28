using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using BlazingOrchard.Liquid.Extensions;
using BlazingOrchard.Liquid.Models;
using Fluid;
using Microsoft.Extensions.Caching.Memory;

namespace BlazingOrchard.Liquid.Services
{
    public class LiquidTemplateManager : ILiquidTemplateManager
    {
        private readonly IMemoryCache _memoryCache;

        public LiquidTemplateManager(IServiceProvider serviceProvider, IMemoryCache memoryCache)
        {
            _memoryCache = memoryCache;
            Context = new LiquidTemplateContextInternal(serviceProvider);
        }

        public LiquidTemplateContext Context { get; }

        public async Task<string?> RenderAsync(string source,
            TextEncoder encoder,
            object model,
            Action<Scope> scopeAction)
        {
            if (string.IsNullOrWhiteSpace(source))
                return null;

            var result = GetCachedTemplate(source);
            return await result.RenderAsync(encoder, Context, model, scopeAction);
        }

        public Task RenderAsync(string source,
            TextWriter writer,
            TextEncoder encoder,
            object model,
            Action<Scope> scopeAction)
        {
            if (string.IsNullOrWhiteSpace(source))
                return Task.CompletedTask;

            var result = GetCachedTemplate(source);
            return result.RenderAsync(writer, encoder, Context, model, scopeAction);
        }

        private LiquidViewTemplate GetCachedTemplate(string source)
        {
            IEnumerable<string> errors;

            var result = _memoryCache.GetOrCreate(
                source,
                e =>
                {
                    if (!LiquidViewTemplate.TryParse(source, out var parsed, out errors))
                    {
                        // If the source string cannot be parsed, create a template that contains the parser errors
                        LiquidViewTemplate.TryParse(
                            string.Join(System.Environment.NewLine, errors),
                            out parsed,
                            out errors);
                    }

                    // Define a default sliding expiration to prevent the
                    // cache from being filled and still apply some micro-caching
                    // in case the template is use commonly
                    e.SetSlidingExpiration(TimeSpan.FromSeconds(30));
                    return parsed;
                });

            return result;
        }

        public bool Validate(string template, out IEnumerable<string> errors) =>
            LiquidViewTemplate.TryParse(template, out _, out errors);
    }
}