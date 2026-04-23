using ApexCharts;
using HITSBlazor.Components.Modals.CenterModals.AddTeamMembersModal;
using HITSBlazor.Components.Skills;
using HITSBlazor.Models.Common.Entities;
using HITSBlazor.Models.Common.Enums;
using HITSBlazor.Models.Teams.Requests;
using HITSBlazor.Models.Users.Entities;
using HITSBlazor.Services.Auth;
using HITSBlazor.Services.Modal;
using HITSBlazor.Services.Skills;
using HITSBlazor.Services.Teams;
using HITSBlazor.Services.UserSkills;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;

namespace HITSBlazor.Pages.Teams.TeamsCreate
{
    [Authorize]
    [Route("/teams/create")]
    [Route("/teams/create/{TeamId}")]
    public partial class TeamsCreate
    {
        [Inject]
        private IAuthService AuthService { get; set; } = null!;

        [Inject]
        private ITeamService TeamsService { get; set; } = null!;

        [Inject]
        private ISkillService SkillService { get; set; } = null!;

        [Inject]
        private IUserSkillService UserSkillService { get; set; } = null!;

        [Inject]
        private ModalService ModalService { get; set; } = null!;

        [Parameter]
        public string TeamId { get; set; } = string.Empty;

        private bool _isLoading = true;

        private string TeamName { get; set; } = string.Empty;
        private string TeamDescription { get; set; } = string.Empty;
        private bool? _teamIsClosed;
        private User? SelectedOwner { get; set; }
        private User? SelectedLeader { get; set; }
        private HashSet<User> SelectedMember { get; set; } = [];
        private List<User> TeamMembers { get; set; } = [];
        private List<User> MembersForInviting { get; set; } = [];

        private List<Skill> LanguageSkills { get; set; } = [];
        private List<Skill> FrameworkSkills { get; set; } = [];
        private List<Skill> DatabaseSkills { get; set; } = [];
        private List<Skill> DevopsSkills { get; set; } = [];

        private HashSet<Skill> SelectedLanguageSkills { get; set; } = [];
        private HashSet<Skill> SelectedFrameworkSkills { get; set; } = [];
        private HashSet<Skill> SelectedDatabaseSkills { get; set; } = [];
        private HashSet<Skill> SelectedDevopsSkills { get; set; } = [];

        private bool WantedSkillsAny => SelectedLanguageSkills.Count > 0 
                                     || SelectedFrameworkSkills.Count > 0 
                                     || SelectedDatabaseSkills.Count > 0 
                                     || SelectedDevopsSkills.Count > 0;

        private readonly Dictionary<SkillType, ApexChartOptions<Skill>> _skillRadarOptions = [];
        private HashSet<Skill> TeamSkills { get; set; } = [];

        protected override async Task OnInitializedAsync()
        {
            _isLoading = true;

            LanguageSkills = await SkillService.GetSkillsAsync(skillTypes: [SkillType.Language]);
            FrameworkSkills = await SkillService.GetSkillsAsync(skillTypes: [SkillType.Framework]);
            DatabaseSkills = await SkillService.GetSkillsAsync(skillTypes: [SkillType.Database]);
            DevopsSkills = await SkillService.GetSkillsAsync(skillTypes: [SkillType.Devops]);

            foreach (var skillType in Enum.GetValues<SkillType>())
                _skillRadarOptions.Add(skillType, GetRadarChartOptions());

            if (Guid.TryParse(TeamId, out Guid guid))
            {
                var team = await TeamsService.GetTeamByIdAsync(guid);
                if (team is not null)
                {
                    TeamName = team.Name;
                    TeamDescription = team.Description;
                    _teamIsClosed = team.Closed;
                    SelectedOwner = new User
                    {
                        Id = team.Owner.UserId,
                        Email = team.Owner.Email,
                        FirstName = team.Owner.FirstName,
                        LastName = team.Owner.LastName
                    };
                    SelectedLeader = team.Leader is not null ? new User
                    {
                        Id = team.Leader.UserId,
                        Email = team.Leader.Email,
                        FirstName = team.Leader.FirstName,
                        LastName = team.Leader.LastName
                    } : null;

                    _isLoading = false;
                }
            }
            else
            {
                if (AuthService.CurrentUser is not null)
                {
                    SelectedOwner = AuthService.CurrentUser;
                    TeamMembers.Add(AuthService.CurrentUser);
                }

                _isLoading = false;
            }
        }

        private static string GetTeamClosedStyle(bool? isClosed) => isClosed.HasValue && isClosed.Value
                ? "team-type-active border-primary border-opacity-75"
                : string.Empty;

        private void ShowInviteUsersModal()
        {
            ModalService.Show<AddTeamMembersModal>(ModalType.Center);
        }

        private async Task UpdateTeamSkills()
        {
            TeamSkills.Clear();
            TeamMembers.ForEach(async m => 
                (await UserSkillService.GetUserSkillsAsync(m.Id)).ForEach(s => 
                    TeamSkills.Add(s)
                )
            );
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
            Fill = new Fill
            {
                Opacity = 0.5,
                Type = new List<FillType> { FillType.Solid }
            },
            Markers = new Markers { Size = 4 },
            Legend = new Legend
            {
                Show = true,
                Position = LegendPosition.Bottom
            }
        };
    }
}
