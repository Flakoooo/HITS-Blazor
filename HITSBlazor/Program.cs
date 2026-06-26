using ApexCharts;
using HITSBlazor.Services;
using HITSBlazor.Services.ActionMenus;
using HITSBlazor.Services.Auth;
using HITSBlazor.Services.Companies;
using HITSBlazor.Services.DragAndDrop;
using HITSBlazor.Services.IdeaMarkets;
using HITSBlazor.Services.Ideas;
using HITSBlazor.Services.Invitation;
using HITSBlazor.Services.Markets;
using HITSBlazor.Services.Modal;
using HITSBlazor.Services.Notifications;
using HITSBlazor.Services.Profiles;
using HITSBlazor.Services.Projects;
using HITSBlazor.Services.Ratings;
using HITSBlazor.Services.Skills;
using HITSBlazor.Services.Tags;
using HITSBlazor.Services.Teams;
using HITSBlazor.Services.TestResults;
using HITSBlazor.Services.Tests;
using HITSBlazor.Services.Users;
using HITSBlazor.Services.UsersGroups;
using HITSBlazor.Services.UserSkills;
using HITSBlazor.Utils.Properties;
using KristofferStrube.Blazor.Popper;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;



#if DEBUGAPI || RELEASE
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
            builder.Services.AddScoped<Popper>();

            // Utils
            builder.Services.AddScoped<GlobalNotificationService>();
            builder.Services.AddScoped<ModalService>();
            builder.Services.AddScoped<ActionMenuService>();
            builder.Services.AddScoped<NavigationService>();
            builder.Services.AddSingleton<DragDropService>();

            // Auth & User & Invitation
#if DEBUG && !DEBUGAPI
            builder.Services.AddScoped<IAuthService, MockAuthService>();
            builder.Services.AddScoped<IUserService, MockUserService>();
            builder.Services.AddScoped<IInvitationService, MockInvitationService>();
#else
            builder.Services.AddScoped<AuthApi>();
            builder.Services.AddScoped<UserApi>();
            builder.Services.AddScoped<InvitationApi>();
            builder.Services.AddScoped<IAuthService, AuthService>();
            builder.Services.AddScoped<IUserService, UserService>();
            builder.Services.AddScoped<IInvitationService, InvitationService>();
#endif
            builder.Services.AddScoped<CustomAuthenticationStateProvider>();
            builder.Services.AddScoped<AuthenticationStateProvider>(sp =>
                sp.GetRequiredService<CustomAuthenticationStateProvider>()
            );
            builder.Services.AddCascadingAuthenticationState();
            builder.Services.AddAuthorizationCore();

            //Skills
#if DEBUG && !DEBUGAPI
            builder.Services.AddScoped<ISkillService, MockSkillService>();
#else
            builder.Services.AddScoped<SkillApi>();
            builder.Services.AddScoped<ISkillService, SkillService>();
#endif

            //Tag
#if DEBUG && !DEBUGAPI
            builder.Services.AddScoped<ITagService, MockTagService>();
#else
            builder.Services.AddScoped<TagApi>();
            builder.Services.AddScoped<ITagService, TagService>();
#endif

            //UserSkills
#if DEBUG && !DEBUGAPI
            builder.Services.AddScoped<IUserSkillService, MockUserSkillService>();
#else
            builder.Services.AddScoped<UserSkillsApi>();
            builder.Services.AddScoped<IUserSkillService, UserSkillService>();
#endif

            //Companies
#if DEBUG && !DEBUGAPI
            builder.Services.AddScoped<ICompanyService, MockCompanyService>();
#else
            builder.Services.AddScoped<CompanyApi>();
            builder.Services.AddScoped<ICompanyService, CompanyService>();
#endif

            //UsersGroups
#if DEBUG && !DEBUGAPI
            builder.Services.AddScoped<IUsersGroupsService, MockUsersGroupsService>();
#else
            builder.Services.AddScoped<UsersGroupApi>();
            builder.Services.AddScoped<IUsersGroupsService, UsersGroupsService>();
#endif

            //Ideas
#if DEBUG && !DEBUGAPI
            builder.Services.AddScoped<IIdeasService, MockIdeasService>();
#else
            builder.Services.AddScoped<IdeaApi>();
            builder.Services.AddScoped<IIdeasService, IdeasService>();
#endif

            //Ratings
#if DEBUG && !DEBUGAPI
            builder.Services.AddScoped<IRatingService, MockRatingService>();
#else
            builder.Services.AddScoped<RatingApi>();
            builder.Services.AddScoped<IRatingService, RatingService>();
#endif

            //Teams
            builder.Services.AddScoped<ITeamService, MockTeamService>();

            //Projects
            builder.Services.AddScoped<IProjectService, MockProjectService>();

            //Market
            builder.Services.AddScoped<IMarketService, MockMarketService>();

            //IdeaMarket
            builder.Services.AddScoped<IIdeaMarketService, MockIdeaMarketService>();

            //Tests
            builder.Services.AddScoped<ITestService, MockTestService>();

            //TestResults
            builder.Services.AddScoped<ITestResultService, MockTestResultService>();

            //Profile
            builder.Services.AddScoped<IProfileService, MockProfileService>();

            //Notification
            builder.Services.AddScoped<INotificationService, MockNotificationService>();

            var host = builder.Build();

            var apexChartService = host.Services.GetRequiredService<IApexChartService>();
            await apexChartService.InitalizeChartAsync();

            await host.RunAsync();
        }
    }
}
