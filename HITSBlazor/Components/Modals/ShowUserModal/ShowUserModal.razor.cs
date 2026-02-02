using ApexCharts;
using HITSBlazor.Components.Modals.ShowIdeaModal;
using HITSBlazor.Components.TableActionMenu;
using HITSBlazor.Models.Common.Entities;
using HITSBlazor.Models.Common.Enums;
using HITSBlazor.Models.Teams.Entities;
using HITSBlazor.Models.Tests.Entities;
using HITSBlazor.Models.Users.Entities;
using HITSBlazor.Models.Users.Enums;
using HITSBlazor.Services.Auth;
using HITSBlazor.Services.Modal;
using HITSBlazor.Services.Profiles;
using HITSBlazor.Services.Skills;
using HITSBlazor.Services.Tests;
using HITSBlazor.Services.UserSkills;
using Microsoft.AspNetCore.Components;

namespace HITSBlazor.Components.Modals.ShowUserModal
{

    //TODO: Это модальное окно многофункциональное, в том плане что вызывается в разных частях сайта, я не конкретной части
    //TODO: путь будет "profile/userId", указать этот путь каждой странице не могу, значит надо как то через срвис навигации вызывать компонент
    //TODO: Если такое реализовать, то модальное окно идей также нужно реализовать
    public partial class ShowUserModal
    {
        [Inject]
        private ModalService ModalService { get; set; } = null!;

        [Inject]
        private IAuthService AuthService { get; set; } = null!;

        [Inject]
        private IProfileService ProfileService { get; set; } = null!;

        [Inject]
        private ISkillService SkillService { get; set; } = null!;

        [Inject]
        private IUserSkillService UserSkillService { get; set; } = null!;

        [Inject]
        private ITestService TestService { get; set; } = null!;

        [Inject] 
        public IApexChartService ApexChartService { get; set; } = null!;

        [Parameter]
        public Guid UserId { get; set; }

        private bool isLoading = true;
        private bool isCurrentUser = false;

        private bool isChangeSkills = false;
        private bool isSkillsLoading = true;

        //нужно как то продемонстрировать процесс загрузки идей, но как, если это одна модель, хмммм
        private bool ideasIsLoading = false;

        private Profile? Profile { get; set; }

        private List<Skill> LanguageSkills { get; set; } = [];
        private List<Skill> FrameworkSkills { get; set; } = [];
        private List<Skill> DatabaseSkills { get; set; } = [];
        private List<Skill> DevopsSkills { get; set; } = [];

        private HashSet<Skill> SelectedLanguageSkills { get; set; } = [];
        private HashSet<Skill> SelectedFrameworkSkills { get; set; } = [];
        private HashSet<Skill> SelectedDatabaseSkills { get; set; } = [];
        private HashSet<Skill> SelectedDevopsSkills { get; set; } = [];

        private TestResult? BelbinTestResult { get; set; }
        private TestResult? TemperTestResult { get; set; }
        private TestResult? MindTestResult { get; set; }

        protected override async Task OnInitializedAsync()
        {
            isLoading = true;

            Profile = await ProfileService.GetUserProifleAsync(UserId);
            if (Profile is null) return;

            BelbinTestResult = await TestService.GetTestResultAsync(UserId, TestService.BelbinTestName);
            TemperTestResult = await TestService.GetTestResultAsync(UserId, TestService.TemperTestName);
            MindTestResult = await TestService.GetTestResultAsync(UserId, TestService.MindTestName);

            if (UserId == AuthService.CurrentUser?.Id)
                isCurrentUser = true;

            isLoading = false;
        }

        private async Task ChangeToUpdateSkills()
        {
            isChangeSkills = true;
            isSkillsLoading = true;
            StateHasChanged();

            LanguageSkills = await SkillService.GetSkillsByTypeAsync(SkillType.Language);
            FrameworkSkills = await SkillService.GetSkillsByTypeAsync(SkillType.Framework);
            DatabaseSkills = await SkillService.GetSkillsByTypeAsync(SkillType.Database);
            DevopsSkills = await SkillService.GetSkillsByTypeAsync(SkillType.Devops);

            SelectedLanguageSkills = Profile?.Skills.Where(s => s.Type == SkillType.Language).ToHashSet() ?? [];
            SelectedFrameworkSkills = Profile?.Skills.Where(s => s.Type == SkillType.Framework).ToHashSet() ?? [];
            SelectedDatabaseSkills = Profile?.Skills.Where(s => s.Type == SkillType.Database).ToHashSet() ?? [];
            SelectedDevopsSkills = Profile?.Skills.Where(s => s.Type == SkillType.Devops).ToHashSet() ?? [];

            isSkillsLoading = false;
        }

        private async Task UpdateUserSkills()
        {
            List<Skill>? newSkills = 
            [
                .. SelectedLanguageSkills, 
                .. SelectedFrameworkSkills, 
                .. SelectedDatabaseSkills, 
                .. SelectedDevopsSkills
            ];
            await UserSkillService.UpdateUserSkillsAsync(UserId, newSkills);
            Profile?.Skills = await UserSkillService.GetUserSkillsAsync(UserId);

            isChangeSkills = false;
        }

        private async Task<List<Skill>> SearchSkillsAsync(SkillType skillType, string searchText)
        {
            if (string.IsNullOrWhiteSpace(searchText))
            {
                return skillType switch
                {
                    SkillType.Language => LanguageSkills,
                    SkillType.Framework => FrameworkSkills,
                    SkillType.Database => DatabaseSkills,
                    SkillType.Devops => DevopsSkills,
                    _ => []
                };
            }

            return await SkillService.GetSkillByTypeAndByNameAsync(skillType, searchText);
        }

        private static ApexChartOptions<Skill> GetRadarChartOptions(SkillType type)
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

        private async Task OnTeamAction(TableActionContext context)
        {
            if (context.Action == TableAction.View)
            {
                Console.WriteLine($"Демонстрация идеи {context.ItemId}");
            }
        }
    }
}
