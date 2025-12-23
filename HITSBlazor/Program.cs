using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using HITSBlazor.Services.Service.Interfaces;
using HITSBlazor.Services.Service.Class;
using HITSBlazor.Utils.Properties;

#if DEBUG && !DEBUGAPI
using HITSBlazor.Services.Service.Mock;
#else
using System.Net.Http.Headers;
using HITSBlazor.Services.Api;
using System.Net.Mime;
#endif

namespace HITSBlazor
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
#if DEBUGAPI
            const string API_BASE_URL = "http://localhost:8080";
#elif RELEASE
            const string API_BASE_URL = "http://localhost:8080";
#endif
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("#app");
            builder.RootComponents.Add<HeadOutlet>("head::after");
#if DEBUGAPI || RELEASE
            builder.Services.AddScoped<CookieHandler>();
            builder.Services.AddHttpClient("HITSClient", client =>
            {
                client.BaseAddress = new Uri(API_BASE_URL);
                client.DefaultRequestHeaders.Accept.Add(
                    new MediaTypeWithQualityHeaderValue(MediaTypeNames.Application.Json));
            }).AddHttpMessageHandler<CookieHandler>();

            builder.Services.AddScoped(sp => 
                sp.GetRequiredService<IHttpClientFactory>().CreateClient("HITSClient")
            );
#endif

            // Utils
            builder.Services.AddScoped<NotificationService>();

            // Auth
#if DEBUG && !DEBUGAPI
            builder.Services.AddScoped<IAuthService, MockAuthService>();
#else
            builder.Services.AddScoped<AuthApi>();
            builder.Services.AddScoped<IAuthService, AuthService>();
#endif
            builder.Services.AddScoped<CustomAuthenticationStateProvider>();
            builder.Services.AddScoped<AuthenticationStateProvider>(sp =>
                sp.GetRequiredService<CustomAuthenticationStateProvider>()
            );
            builder.Services.AddCascadingAuthenticationState();
            builder.Services.AddAuthorizationCore();

            await builder.Build().RunAsync();
        }
    }
}
