using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Fluid;

namespace BlazingOrchard.Liquid.Services
{
    public class LiquidTemplateManager : ILiquidTemplateManager
    {
        public LiquidTemplateManager(IServiceProvider serviceProvider)
        {
            Context = new LiquidTemplateContext(serviceProvider);
        }
        
        public LiquidTemplateContext Context { get; }

        public Task<string?> RenderAsync(string source, TextEncoder encoder, object model, Action<Scope> scopeAction)
        {
            if (string.IsNullOrWhiteSpace(source))
                return Task.FromResult((string?)null);

            var result = GetCachedTemplate(source);

            return result.RenderAsync(encoder, Context, model, scopeAction);
        }

        public Task RenderAsync(string source, TextWriter writer, TextEncoder encoder, object model, Action<Scope> scopeAction)
        {
            if (string.IsNullOrWhiteSpace(source))
            {
                return Task.CompletedTask;
            }

            var result = GetCachedTemplate(source);

            return result.RenderAsync(writer, encoder, Context, model, scopeAction);
        }

        private LiquidViewTemplate GetCachedTemplate(string source)
        {
            var errors = Enumerable.Empty<string>();

            var result = _memoryCache.GetOrCreate(source, (ICacheEntry e) =>
            {
                if (!LiquidViewTemplate.TryParse(source, out var parsed, out errors))
                {
                    // If the source string cannot be parsed, create a template that contains the parser errors
                    LiquidViewTemplate.TryParse(string.Join(System.Environment.NewLine, errors), out parsed, out errors);
                }

                // Define a default sliding expiration to prevent the
                // cache from being filled and still apply some micro-caching
                // in case the template is use commonly
                e.SetSlidingExpiration(TimeSpan.FromSeconds(30));
                return parsed;
            });

            return result;
        }

        public bool Validate(string template, out IEnumerable<string> errors)
        {
            return LiquidViewTemplate.TryParse(template, out _, out errors);
        }
    }
}
