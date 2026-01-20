using ApexCharts;
using HITSBlazor.Models.Common.Entities;
using HITSBlazor.Models.Tests.Entities;
using HITSBlazor.Models.Users.Entities;
using HITSBlazor.Models.Users.Enums;
using HITSBlazor.Services.Auth;
using HITSBlazor.Services.Profiles;
using HITSBlazor.Services.Tests;
using Microsoft.AspNetCore.Components;

namespace HITSBlazor.Components.Modals.ShowUserModal
{

    //TODO: Это модальное окно многофункциональное, в том плане что вызывается в разных частях сайта, я не конкретной части
    //TODO: путь будет "profile/userId", указать этот путь каждой странице не могу, значит надо как то через срвис навигации вызывать компонент
    //TODO: Если такое реализовать, то модальное окно идей также нужно реализовать
    public partial class ShowUserModal
    {
        [Inject]
        private IAuthService AuthService { get; set; } = null!;

        [Inject]
        private IProfileService ProfileService { get; set; } = null!;

        [Inject]
        private ITestService TestService { get; set; } = null!;

        [Inject] 
        public IApexChartService ApexChartService { get; set; } = null!;

        [Parameter]
        public Guid UserId { get; set; }

        private bool isLoading = true;
        //нужно как то продемонстрировать процесс загрузки идей, но как, если это одна модель, хмммм
        private bool ideasIsLoading = false;

        private Profile? Profile { get; set; }
        private RoleType? CurrentUserRole { get; set; }

        private ApexChartOptions<Skill> Options = new();

        private TestResult? BelbinTestResult { get; set; }
        private TestResult? TemperTestResult { get; set; }
        private TestResult? MindTestResult { get; set; }

        protected override async Task OnInitializedAsync()
        {
            isLoading = true;

            Profile = await ProfileService.GetUserProifleAsync(UserId);
            if (Profile is null) return;

            var globalOptions = ApexChartService.GlobalOptions;
            Options.Annotations = globalOptions.Annotations;
            Options.Blazor = globalOptions.Blazor;
            Options.Chart = globalOptions.Chart;
            Options.Colors = globalOptions.Colors;
            Options.DataLabels = globalOptions.DataLabels;
            Options.Debug = globalOptions.Debug;
            Options.Fill = globalOptions.Fill;
            Options.ForecastDataPoints = globalOptions.ForecastDataPoints;
            Options.Grid = globalOptions.Grid;
            Options.Labels = globalOptions.Labels;
            Options.Legend = globalOptions.Legend;
            Options.Markers = globalOptions.Markers;
            Options.NoData = globalOptions.NoData;
            Options.PlotOptions = globalOptions.PlotOptions;
            Options.States = globalOptions.States;
            Options.Stroke = globalOptions.Stroke;
            Options.Subtitle = globalOptions.Subtitle;
            Options.Theme = globalOptions.Theme;
            Options.Title = globalOptions.Title;
            Options.Tooltip = globalOptions.Tooltip;
            Options.Xaxis = globalOptions.Xaxis;
            Options.Yaxis = globalOptions.Yaxis;

            BelbinTestResult = await TestService.GetTestResultAsync(UserId, TestService.BelbinTestName);
            TemperTestResult = await TestService.GetTestResultAsync(UserId, TestService.TemperTestName);
            MindTestResult = await TestService.GetTestResultAsync(UserId, TestService.MindTestName);

            var currentUser = AuthService.CurrentUser;
            if (UserId == currentUser?.Id)
                CurrentUserRole = currentUser.Role;

            isLoading = false;
        }
    }
}
