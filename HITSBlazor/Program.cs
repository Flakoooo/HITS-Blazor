using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using HITSBlazor.Services.Service.Interfaces;
using HITSBlazor.Services.Service.Class;
using System.Net.Http.Headers;
using System.Net.Mime;
using Microsoft.AspNetCore.Components.WebAssembly.Http;


#if DEBUG
using HITSBlazor.Services.Service.Mock;
#else
using HITSBlazor.Services.Api;
#endif

namespace HITSBlazor
{
    public class CookieHandler : DelegatingHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            request.SetBrowserRequestCredentials(BrowserRequestCredentials.Include);
            return base.SendAsync(request, cancellationToken);
        }
    }

    public class Program
    {
        public static async Task Main(string[] args)
        {
#if DEBUG
            const string API_BASE_URL = "http://localhost:8080";
#else
            const string API_BASE_URL = "http://localhost:8080";
#endif

            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("#app");
            builder.RootComponents.Add<HeadOutlet>("head::after");

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

            // Utils
            builder.Services.AddScoped<NotificationService>();

            // Auth
#if DEBUG
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
