using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using HITSBlazor.Services.Service.Interfaces;
using HITSBlazor.Services.Service.Class;
using Blazored.LocalStorage;
using System.Net.Http.Headers;


#if DEBUG
using HITSBlazor.Services.Service.Mock;
#else
using HITSBlazor.Services.Api;
#endif
using HITSBlazor.Utils;

namespace HITSBlazor
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
#if DEBUG
            const string API_BASE_URL = "http://localhost:8080/api";
#else
            const string API_BASE_URL = "http://localhost:8080/api";
#endif

            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("#app");
            builder.RootComponents.Add<HeadOutlet>("head::after");

            builder.Services.AddBlazoredLocalStorage();

            builder.Services.AddScoped<AuthHeaderHandler>();

            builder.Services.AddHttpClient("ApiClient", client =>
            {
                client.BaseAddress = new Uri(API_BASE_URL);
                client.DefaultRequestHeaders.Accept.Add(
                    new MediaTypeWithQualityHeaderValue("application/json"));
            })
            .AddHttpMessageHandler<AuthHeaderHandler>();
            builder.Services.AddScoped(sp =>
            {
                var factory = sp.GetRequiredService<IHttpClientFactory>();
                return factory.CreateClient("ApiClient");
            });

            // Auth
#if DEBUG
            builder.Services.AddScoped<IAuthService, MockAuthService>();
#else
            builder.Services.AddScoped<IAuthService, AuthService>();
            builder.Services.AddScoped<AuthApi>();
#endif
            // Utils
            builder.Services.AddScoped<ICookieService, CookieService>();
            builder.Services.AddScoped<NotificationService>();


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
