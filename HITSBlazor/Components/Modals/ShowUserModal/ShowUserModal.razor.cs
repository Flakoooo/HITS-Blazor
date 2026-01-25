using ApexCharts;
using HITSBlazor.Models.Common.Entities;
using HITSBlazor.Models.Common.Enums;
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

        private TestResult? BelbinTestResult { get; set; }
        private TestResult? TemperTestResult { get; set; }
        private TestResult? MindTestResult { get; set; }

        private ApexChartOptions<Skill> GetRadarChartOptions(SkillType type)
        {
            return new ApexChartOptions<Skill>
            {
                Chart = new Chart
                {
                    Type = ChartType.Radar,
                    Toolbar = new Toolbar { Show = false }
                },
                Yaxis =
                [
                    new YAxis
                    {
                        Min = 0,
                        Max = 1,
                        Labels = new YAxisLabels { Show = false },
                        Show = false
                    }
                ],
                Xaxis = new XAxis
                {
                    Labels = new XAxisLabels
                    {
                        Style = new AxisLabelStyle
                        {
                            Colors = new List<string> { "#a8a8a8" },
                            FontSize = "11px"
                        }
                    }
                },
                Stroke = new Stroke { Width = 2 },
                Fill = new Fill { Opacity = 0.1 },
                Markers = new Markers { Size = 4 },
                Legend = new Legend
                {
                    Show = true,
                    Position = LegendPosition.Bottom
                }
            };
        }

        protected override async Task OnInitializedAsync()
        {
            isLoading = true;

            Profile = await ProfileService.GetUserProifleAsync(UserId);
            if (Profile is null) return;

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
