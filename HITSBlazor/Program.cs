using HITSBlazor.Services;
using HITSBlazor.Utils;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

namespace HITSBlazor
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
#if DEBUG
            const bool USE_MOCK_DATA = true;
            const string API_BASE_URL = "https://localhost:5001";
#else
            const bool USE_MOCK_DATA = false; 
            const string API_BASE_URL = "https://api.yourserver.com";
#endif

            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("#app");
            builder.RootComponents.Add<HeadOutlet>("head::after");

            builder.Services.AddSingleton(new AppSettings
            {
                UseMockData = USE_MOCK_DATA,
                ApiBaseUrl = API_BASE_URL,
                Environment = USE_MOCK_DATA ? "Development" : "Production"
            });

            builder.Services.AddScoped(sp => new HttpClient { 
                BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) 
            });

            // Auth
#if DEBUG
            builder.Services.AddScoped<IAuthService, MockAuthService>();
#else
            builder.Services.AddScoped<IAuthService, AuthService>();
#endif
            // Utils
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
