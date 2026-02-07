using ApexCharts;
using HITSBlazor.Components.Modals.CenterModals.UpdateEmailModal;
using HITSBlazor.Components.Modals.RightSideModals.ShowTeamModal;
using HITSBlazor.Components.Tables.TableActionMenu;
using HITSBlazor.Components.Tables.TableHeader;
using HITSBlazor.Models.Common.Entities;
using HITSBlazor.Models.Common.Enums;
using HITSBlazor.Models.Ideas.Entities;
using HITSBlazor.Models.Tests.Entities;
using HITSBlazor.Models.Users.Entities;
using HITSBlazor.Services.Auth;
using HITSBlazor.Services.Modal;
using HITSBlazor.Services.Profiles;
using HITSBlazor.Services.Skills;
using HITSBlazor.Services.Tests;
using HITSBlazor.Services.UserSkills;
using Microsoft.AspNetCore.Components;

namespace HITSBlazor.Components.Modals.RightSideModals.ShowUserModal
{
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

        private bool _isLoading = true;
        private bool _isCurrentUser = false;

        private string? _editingField;

        private bool _isChangeSkills = false;
        private bool _isSkillsLoading = true;

        //нужно как то продемонстрировать процесс загрузки идей, но как, если это одна модель, хмммм
        private bool ideasIsLoading = false;

        private Profile? Profile { get; set; }
        private UserDataForm? _userDataForm;

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
            _isLoading = true;

            Profile = await ProfileService.GetUserProifleAsync(UserId);
            if (Profile is null) return;

            _userDataForm = ResetUserForm(Profile);

            BelbinTestResult = await TestService.GetTestResultAsync(UserId, TestService.BelbinTestName);
            TemperTestResult = await TestService.GetTestResultAsync(UserId, TestService.TemperTestName);
            MindTestResult = await TestService.GetTestResultAsync(UserId, TestService.MindTestName);

            if (UserId == AuthService.CurrentUser?.Id)
                _isCurrentUser = true;

            AuthService.OnAuthStateChanged += _isCurrentUser ? UpdateCurrentProfile : null;

            _isLoading = false;
        }

        private static UserDataForm ResetUserForm(Profile original) => new()
        {
            FirstName = original.FirstName,
            LastName = original.LastName,
            Telephone = original.Telephone,
            StudyGroup = original.StudyGroup
        };

        private void UpdateCurrentProfile()
        {
            var user = AuthService.CurrentUser;
            if (user is null || Profile is null) return;

            Profile.Email = user.Email;
            _userDataForm?.FirstName = Profile.FirstName = user.FirstName;
            _userDataForm?.LastName = Profile.LastName = user.LastName;
            _userDataForm?.Telephone = Profile.Telephone = user.Telephone;
            _userDataForm?.StudyGroup = Profile.StudyGroup = user.StudyGroup;
            Profile.Roles = user.Roles;

            StateHasChanged();
        }

        private void StartEdit(string fieldName)
        {
            _editingField = fieldName;

            if (Profile is null) return;
            _userDataForm = ResetUserForm(Profile);
        }

        private async Task SaveEdit()
        {
            if (Profile is not null && _editingField is not null && _userDataForm is not null)
            {
                await ProfileService.UpdateProfileUserDataAsync(_userDataForm);
            }
            _editingField = null;
        }

        private void CancelEdit()
        {
            _editingField = null;

            if (Profile is null) return;
            _userDataForm = ResetUserForm(Profile);
        }

        private async Task ChangeToUpdateSkills()
        {
            _isChangeSkills = true;
            _isSkillsLoading = true;
            StateHasChanged();

            LanguageSkills = await SkillService.GetSkillsByTypeAsync(SkillType.Language);
            FrameworkSkills = await SkillService.GetSkillsByTypeAsync(SkillType.Framework);
            DatabaseSkills = await SkillService.GetSkillsByTypeAsync(SkillType.Database);
            DevopsSkills = await SkillService.GetSkillsByTypeAsync(SkillType.Devops);

            SelectedLanguageSkills = Profile?.Skills.Where(s => s.Type == SkillType.Language).ToHashSet() ?? [];
            SelectedFrameworkSkills = Profile?.Skills.Where(s => s.Type == SkillType.Framework).ToHashSet() ?? [];
            SelectedDatabaseSkills = Profile?.Skills.Where(s => s.Type == SkillType.Database).ToHashSet() ?? [];
            SelectedDevopsSkills = Profile?.Skills.Where(s => s.Type == SkillType.Devops).ToHashSet() ?? [];

            _isSkillsLoading = false;
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

            _isChangeSkills = false;
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

        private static ApexChartOptions<Skill> GetRadarChartOptions() => new()
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

        private static List<TableHeaderItem> GetHeaderItems() =>
        [
            new TableHeaderItem
            {
                Text = "Команда"
            },
            new TableHeaderItem
            {
                Text = "Дата вступления",
                InCentered = true
            },
            new TableHeaderItem
            {
                Text = "Дата исключения",
                InCentered = true
            },
            new TableHeaderItem
            {
                Text = "Статус",
                InCentered = true
            },
        ];

        private void ShowUpdateEmail()
        {
            ModalService.Show<UpdateEmailModal>(ModalType.Center);
        }

        private void ShowIdea(Guid ideaId)
        {
            var modalParameters = new Dictionary<string, object>
            {
                { "IdeaId", ideaId }
            };
            ModalService.Show<ShowIdeaModal.ShowIdeaModal>(type: ModalType.RightSide, parameters: modalParameters);
        }

        private void ShowTeam(Guid teamId)
        {
            var modalParameters = new Dictionary<string, object>
            {
                { "TeamId", teamId }
            };
            ModalService.Show<ShowTeamModal.ShowTeamModal>(type: ModalType.RightSide, parameters: modalParameters);
        }

        private async Task OnTeamAction(TableActionContext context)
        {
            if (context.Action == TableAction.View)
            {
                ShowTeam(context.ItemId);
            }
        }
    }
}
