using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Text.Encodings.Web;
using BlazingOrchard.Extensions;
using Microsoft.Extensions.Caching.Memory;
using BlazingOrchard;

namespace BlazorOIDC
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            var services = builder.Services;
            builder.RootComponents.Add<App>("#app");
            //builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
            builder.Services.AddHttpClient("client")
                .AddHttpMessageHandler(sp =>
                {
                    var handler = sp.GetService<AuthorizationMessageHandler>()
                        .ConfigureHandler(
                            authorizedUrls: new[] { "https://localhost:44300" },
                            scopes: new[] { "openid", "profile", "api" });

                    return handler;
                });
            builder.Services.AddScoped(sp => sp.GetService<IHttpClientFactory>().CreateClient("client"));
            builder.Services.AddOidcAuthentication(options =>
            {
                // Configure your authentication provider options here.
                // For more information, see https://aka.ms/blazor-standalone-auth
                builder.Configuration.Bind("oidc", options.ProviderOptions);
            });
            services
                .AddSingleton<IMemoryCache, MemoryCache>()
                .AddSingleton<HtmlEncoder, DefaultHtmlEncoder>()
                .AddSingleton<IConfiguration>(builder.Configuration)
                .AddBlazingOrchard()
                .AddModules(Modules.GetAssemblies());
            await builder.Build().RunAsync();
        }
    }

    public class DefaultHtmlEncoder : HtmlEncoder
    {
        public override unsafe int FindFirstCharacterToEncode(char* text, int textLength)
        {
            throw new System.NotImplementedException();
        }

        public override unsafe bool TryEncodeUnicodeScalar(int unicodeScalar, char* buffer, int bufferLength, out int numberOfCharactersWritten)
        {
            throw new System.NotImplementedException();
        }

        public override bool WillEncode(int unicodeScalar)
        {
            throw new System.NotImplementedException();
        }

        public override int MaxOutputCharactersPerInputCharacter { get; }
    }
}
