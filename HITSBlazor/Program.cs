using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using HITSBlazor.Utils.Properties;
using HITSBlazor.Services.Auth;
using HITSBlazor.Services;
using HITSBlazor.Services.Notifications;
using HITSBlazor.Services.Ideas;
using HITSBlazor.Services.Skills;
using HITSBlazor.Services.Companies;
using HITSBlazor.Services.Modal;
using HITSBlazor.Services.Markets;
using HITSBlazor.Services.Projects;
using HITSBlazor.Services.IdeaSkills;
using HITSBlazor.Services.IdeaRatings;
using HITSBlazor.Services.Profiles;
using HITSBlazor.Services.Tests;
using ApexCharts;



#if DEBUGAPI || RELEASE
using HITSBlazor.Services.Users;
using HITSBlazor.Services.Invitation;
using HITSBlazor.Utils;
using System.Net.Http.Headers;
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
            builder.Services.AddHttpClient(Settings.HttpClientName, client =>
            {
                client.BaseAddress = new Uri(API_BASE_URL);
                client.DefaultRequestHeaders.Accept.Add(
                    new MediaTypeWithQualityHeaderValue(MediaTypeNames.Application.Json));
            }).AddHttpMessageHandler<CookieHandler>();

            builder.Services.AddScoped(sp => 
                sp.GetRequiredService<IHttpClientFactory>().CreateClient(Settings.HttpClientName)
            );
#endif
            builder.Services.AddApexCharts();

            // Utils
            builder.Services.AddScoped<GlobalNotificationService>();
            builder.Services.AddScoped<ModalService>();
            builder.Services.AddScoped<NavigationService>();

            // Auth
#if DEBUG && !DEBUGAPI
            builder.Services.AddScoped<IAuthService, MockAuthService>();
#else
            builder.Services.AddScoped<AuthApi>();
            builder.Services.AddScoped<UserApi>();
            builder.Services.AddScoped<InvitationApi>();
            builder.Services.AddScoped<IAuthService, AuthService>();
#endif
            builder.Services.AddScoped<CustomAuthenticationStateProvider>();
            builder.Services.AddScoped<AuthenticationStateProvider>(sp =>
                sp.GetRequiredService<CustomAuthenticationStateProvider>()
            );
            builder.Services.AddCascadingAuthenticationState();
            builder.Services.AddAuthorizationCore();

            //Skills
            builder.Services.AddScoped<ISkillService, MockSkillService>();

            //Companies
            builder.Services.AddScoped<ICompanyService, MockCompanyService>();

            //Ideas
            builder.Services.AddScoped<IIdeasService, MockIdeasService>();
            builder.Services.AddScoped<IIdeaSkillService, MockIdeaSkillService>();
            builder.Services.AddScoped<IIdeaRatingService, MockIdeaRatingService>();

            //Projects
            builder.Services.AddScoped<IProjectService, MockProjectService>();

            //Market
            builder.Services.AddScoped<IMarketService, MockMarketService>();

            //Tests
            builder.Services.AddScoped<ITestService, MockTestService>();

            //Profile
            builder.Services.AddScoped<IProfileService, MockProfileService>();

            // Notification
            builder.Services.AddScoped<INotificationService, MockNotificationService>();

            await builder.Build().RunAsync();
        }
    }
}
