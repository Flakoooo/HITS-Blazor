using ApexCharts;
using HITSBlazor.Components.ActionMenus.BaseActionMenu;
using HITSBlazor.Components.Modals.CenterModals.AddTeamMembersModal;
using HITSBlazor.Components.Modals.RightSideModals.ProfileModal;
using HITSBlazor.Components.Tables.TableHeader;
using HITSBlazor.Components.Typography;
using HITSBlazor.Models.Common.Entities;
using HITSBlazor.Models.Common.Enums;
using HITSBlazor.Models.Teams.Entities;
using HITSBlazor.Models.Teams.Requests;
using HITSBlazor.Models.Users.Entities;
using HITSBlazor.Models.Users.Enums;
using HITSBlazor.Pages.Admin.InviteUsers;
using HITSBlazor.Services;
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
    public partial class TeamsCreate : IDisposable
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

        [Inject]
        private NavigationService NavigationService { get; set; } = null!;

        [Parameter]
        public string TeamId { get; set; } = string.Empty;

        private bool _isLoading = true;

        private readonly Dictionary<string, string> _errors = [];

        private Team? _teamForUpdate;

        private string TeamName { get; set; } = string.Empty;
        private string TeamDescription { get; set; } = string.Empty;
        private bool? _teamIsClosed;
        private User? SelectedOwner { get; set; }
        private User? SelectedLeader { get; set; }
        private List<User> TeamMembers { get; set; } = [];
        private List<User> MembersForInviting { get; set; } = [];
        private HashSet<Guid> MembersForKicking { get; set; } = [];
        private HashSet<Skill> MembersForInvitingSkills { get; set; } = [];

        private HashSet<Skill> SelectedLanguageSkills { get; set; } = [];
        private HashSet<Skill> SelectedFrameworkSkills { get; set; } = [];
        private HashSet<Skill> SelectedDatabaseSkills { get; set; } = [];
        private HashSet<Skill> SelectedDevopsSkills { get; set; } = [];

        private bool WantedSkillsAny => SelectedLanguageSkills.Count > 0 
                                     || SelectedFrameworkSkills.Count > 0 
                                     || SelectedDatabaseSkills.Count > 0 
                                     || SelectedDevopsSkills.Count > 0;

        private static List<TableHeaderItem> MembersTableHeader { get; } =
        [
            new() { Text = "Почта",     ColumnClass = "col-5" },
            new() { Text = "Имя",       ColumnClass = "col-3" },
            new() { Text = "Фамилия",   ColumnClass = "col-3" }
        ];

        private readonly Dictionary<SkillType, ApexChartOptions<Skill>> _skillRadarOptions = [];
        private HashSet<Skill> TeamSkills { get; set; } = [];

        protected override async Task OnInitializedAsync()
        {
            _isLoading = true;

            TeamsService.OnInviteMembersCollected += SetTeamInvitationMembers;

            foreach (var skillType in Enum.GetValues<SkillType>())
                _skillRadarOptions.Add(skillType, GetRadarChartOptions());

            if (Guid.TryParse(TeamId, out Guid guid))
            {
                var team = await TeamsService.GetTeamByIdAsync(guid);
                if (team is not null)
                {
                    _teamForUpdate = team;

                    TeamName = team.Name;
                    TeamDescription = team.Description;
                    _teamIsClosed = team.Closed;

                    SelectedLanguageSkills = [.. team.WantedSkills.Where(s => s.Type == SkillType.Language)];
                    SelectedFrameworkSkills = [.. team.WantedSkills.Where(s => s.Type == SkillType.Framework)];
                    SelectedDatabaseSkills = [.. team.WantedSkills.Where(s => s.Type == SkillType.Database)];
                    SelectedDevopsSkills = [.. team.WantedSkills.Where(s => s.Type == SkillType.Devops)];


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

                    TeamMembers = team.Members.Select(tm => new User
                    {
                        Id = tm.UserId,
                        Email = tm.Email,
                        FirstName = tm.FirstName,
                        LastName = tm.LastName
                    }).ToList();

                    TeamSkills = team.Skills.ToHashSet();

                    _isLoading = false;
                    StateHasChanged();
                }
            }
            else
            {
                var currentUser = AuthService.CurrentUser;
                if (currentUser is not null)
                {
                    SelectedOwner = currentUser;
                    SelectedLeader = currentUser;
                    TeamMembers.Add(currentUser);

                    TeamSkills = (await UserSkillService.GetUserSkillsAsync(currentUser.Id)).ToHashSet();
                }

                _isLoading = false;
            }
        }

        private bool SelectOwnerAllowed()
        {
            var currentUser = AuthService.CurrentUser;
            if (currentUser is null) return false;

            if (_teamForUpdate is not null && (currentUser.Id == SelectedOwner?.Id || currentUser.Role is RoleType.Admin))
                return true;

            return false;
        }

        private bool SelectLeaderAllowed()
        {
            var currentUser = AuthService.CurrentUser;
            if (currentUser is null) return false;

            if (_teamForUpdate is not null && (currentUser.Id == SelectedOwner?.Id || currentUser.Id == SelectedLeader?.Id || currentUser.Role is RoleType.Admin))
                return true;

            return false;
        }

        private static string GetTeamClosedStyle(bool? isClosed) => isClosed.HasValue && isClosed.Value
                ? "team-type-active border-primary border-opacity-75"
                : string.Empty;

        private (TextColor? TextColor, string DisplayText) GetRoleDisplayInfo(Guid memberId)
        {
            if (memberId == SelectedOwner?.Id)
            {
                string displayText = "(Владелец)";
                if (memberId == SelectedLeader?.Id)
                    displayText = "(Владелец и Тим-лид)";

                return (TextColor.Warning, displayText);
            }
            else if (memberId == SelectedLeader?.Id)
            {
                return (TextColor.Primary, "(Тим-лид)");
            }
            else if (MembersForKicking.Contains(memberId))
            {
                return (TextColor.Danger, "(Будет исключен)");
            }
            else
            {
                return (null, string.Empty);
            }
        }

        private Dictionary<MenuAction, object> GetActonMenus(User user)
        {
            var actions = new Dictionary<MenuAction, object>();
            if (_teamForUpdate is not null)
            {
                actions.Add(MenuAction.ViewProfile, user.Id);

                var currentUser = AuthService.CurrentUser;
                if (currentUser is not null
                    && (currentUser.Role is RoleType.Admin || _teamForUpdate.Owner.Id == currentUser.Id || _teamForUpdate.Leader?.Id == currentUser.Id))
                {
                    if (MembersForKicking.Contains(user.Id))
                    {
                        actions.Add(MenuAction.TeamMemberRestore, user.Id);
                    }
                    else
                    {
                        if (user.Id == SelectedLeader?.Id && user.Id != SelectedOwner?.Id)
                            actions.Add(MenuAction.UnsetLeader, user.Id);
                        else if (user.Id != SelectedOwner?.Id)
                            actions.Add(MenuAction.SetLeader, user);

                        if (user.Id != SelectedOwner?.Id && !MembersForKicking.Contains(user.Id))
                            actions.Add(MenuAction.RemoveTeamMember, user.Id);
                    }
                }
            }

            return actions;
        }

        private void ShowUserProfile(Guid userId) => ModalService.ShowProfileModal(userId);

        private void HandleTableMenuClick(TableActionContext context)
        {
            if (context.Action is MenuAction.UnsetLeader)
            {
                if (_teamForUpdate is not null)
                {
                    SelectedLeader = new User
                    {
                        Id = _teamForUpdate.Owner.UserId,
                        Email = _teamForUpdate.Owner.Email,
                        FirstName = _teamForUpdate.Owner.FirstName,
                        LastName = _teamForUpdate.Owner.LastName
                    };
                }
            }
            else if (context.Item is Guid userId)
            {
                if (context.Action is MenuAction.ViewProfile)
                {
                    ShowUserProfile(userId);
                }
                else if (context.Action is MenuAction.RemoveTeamMember)
                {
                    MembersForKicking.Add(userId);
                    if (_teamForUpdate is not null && userId == SelectedLeader?.Id)
                    {
                        SelectedLeader = new User
                        {
                            Id = _teamForUpdate.Owner.UserId,
                            Email = _teamForUpdate.Owner.Email,
                            FirstName = _teamForUpdate.Owner.FirstName,
                            LastName = _teamForUpdate.Owner.LastName
                        };
                    }

                    StateHasChanged();
                }
                else if (context.Action is MenuAction.TeamMemberRestore)
                {
                    MembersForKicking.Remove(userId);
                    StateHasChanged();
                }
            }
            else if (context.Item is User user)
            {
                if (context.Action is MenuAction.SetLeader)
                {
                    SelectedLeader = user;
                    StateHasChanged();
                }
                else if (context.Action is MenuAction.TeamRequestCancel)
                {
                    MembersForInviting.Remove(user);
                }
            }
        }

        private async Task CreateTeam()
        {
            _errors.Clear();

            if (string.IsNullOrWhiteSpace(TeamName))
                _errors.Add("name", "Поле не может быть пустым");

            if (string.IsNullOrWhiteSpace(TeamDescription))
                _errors.Add("description", "Поле не может быть пустым");

            if (!_teamIsClosed.HasValue)
                _errors.Add("closed", "Тип команды не выбран");

            if (SelectedOwner is null)
                _errors.Add("owner", "Владелец команды не выбран");

            if (SelectedLeader is null)
                _errors.Add("leader", "Лидер команды не выбран");

            if (_errors.Count > 0) return;

            var request = new CreateTeamRequest
            {
                Name = TeamName,
                Description = TeamDescription,
                IsClosed = _teamIsClosed!.Value,
                OwnerId = SelectedOwner!.Id,
                InvitedMembers = MembersForInviting.Select(m => m.Id).ToList(),
                WantedSkills = 
                [
                    .. SelectedLanguageSkills.Select(s => s.Id),
                    .. SelectedFrameworkSkills.Select(s => s.Id),
                    .. SelectedDatabaseSkills.Select(s => s.Id),
                    .. SelectedDevopsSkills.Select(s => s.Id)
                ]
            };

            if (await TeamsService.CreateTeamAsync(request))
                await NavigationService.NavigateToAsync("/teams/list");

        }

        private void ShowInviteUsersModal() => ModalService.ShowInviteUsersModal(
            MembersForInviting.Concat(TeamMembers).Select(u => u.Id).Distinct().ToHashSet()
        );

        private async void SetTeamInvitationMembers(ICollection<User> users)
        {
            MembersForInviting = MembersForInviting.Concat(users).DistinctBy(u => u.Id).ToList();
            MembersForInvitingSkills.Clear();

            foreach (var member in MembersForInviting)
            {
                var userSkills = await UserSkillService.GetUserSkillsAsync(member.Id);
                foreach (var skill in userSkills)
                    MembersForInvitingSkills.Add(skill);
            }

            StateHasChanged();
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

        public void Dispose()
        {
            TeamsService.OnInviteMembersCollected -= SetTeamInvitationMembers;
        }
    }
}
