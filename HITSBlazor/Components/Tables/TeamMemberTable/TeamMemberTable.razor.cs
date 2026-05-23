using HITSBlazor.Components.ActionMenus.BaseActionMenu;
using HITSBlazor.Components.Button;
using HITSBlazor.Components.Tables.TableHeader;
using HITSBlazor.Components.Typography;
using HITSBlazor.Models.Teams.Entities;
using HITSBlazor.Models.Users.Entities;
using HITSBlazor.Models.Users.Enums;
using HITSBlazor.Services.Auth;
using HITSBlazor.Services.Modal;
using HITSBlazor.Services.Teams;
using Microsoft.AspNetCore.Components;

namespace HITSBlazor.Components.Tables.TeamMemberTable
{
    public partial class TeamMemberTable
    {
        [Inject]
        private AuthService AuthService { get; set; } = null!;

        [Inject]
        private ITeamService TeamService { get; set; } = null!;

        [Inject]
        private ModalService ModalService { get; set; } = null!;

        [Parameter]
        public required Team CurrentTeam { get; set; }

        private bool _isLoading = true;
        private TableComponent.TableComponent? _tableComponent;

        private string _searchText = string.Empty;
        private readonly List<TeamMember> _teamMembers = [];

        private static List<TableHeaderItem> MembersTableHeader { get; } =
        [
            new() { Text = "Почта", ColumnClass = "col-5" },
            new() { Text = "Имя", ColumnClass = "col-3" },
            new() { Text = "Фамилия", ColumnClass = "col-3" }
        ];

        protected override async Task OnInitializedAsync()
        {
            _isLoading = true;

            await LoadTeamMembersAsync();

            _isLoading = false;
            MarkAsInitialized();
        }

        protected override async Task AdditionalAfterRenderMethod()
        {
            if (_tableComponent != null)
                _tableContainer = _tableComponent.ScrollContainer;
        }

        protected override async Task OnLoadMoreItemsAsync() => await LoadTeamMembersAsync(true);

        protected override int GetCurrentItemsCount() => _teamMembers.Count;

        private async Task LoadTeamMembersAsync(bool append = false)
        {
            await LoadDataAsync(
                _teamMembers,
                () => TeamService.GetTeamMembersAsync(
                    _currentPage,
                    teamId: CurrentTeam.Id,
                    searchText: _searchText
                ),
                append
            );
        }

        private async Task SearchData(string value)
        {
            _searchText = value;
            ResetPagination();
            await LoadTeamMembersAsync();
        }

        private (TextColor? TextColor, string DisplayText) GetRoleDisplayInfo(Guid memberId)
        {
            if (memberId == CurrentTeam.Owner.Id)
            {
                string displayText = "(Владелец)";
                if (memberId == CurrentTeam.Leader?.Id)
                    displayText = "(Владелец и Тим-лид)";

                return (TextColor.Warning, displayText);
            }
            else if (memberId == CurrentTeam.Leader?.Id)
            {
                return (TextColor.Primary, "(Тим-лид)");
            }
            else
            {
                return (null, string.Empty);
            }
        }

        private Dictionary<MenuAction, object> GetActions(TeamMember member)
        {
            var actions = new Dictionary<MenuAction, object>
            {
                [MenuAction.ViewProfile] = member.UserId
            };

            var currentUser = AuthService.CurrentUser;
            if (currentUser is not null)
            {
                if (currentUser.Role is RoleType.Admin || currentUser.Id == CurrentTeam.Owner.Id || currentUser.Id == CurrentTeam.Leader?.Id)
                {
                    if (member.UserId == CurrentTeam.Leader?.Id && member.UserId != CurrentTeam.Owner.Id)
                        actions.Add(MenuAction.UnsetLeader, member.UserId);
                    else if (member.UserId != CurrentTeam.Owner.Id)
                        actions.Add(MenuAction.SetLeader, member);

                    if (member.UserId != CurrentTeam.Owner.Id)
                        actions.Add(MenuAction.RemoveTeamMember, member);
                }
            }

            return actions;
        }

        private void ShowUserProfile(Guid userId) => ModalService.ShowProfileModal(userId);

        private void HandleTableMenuClick(TableActionContext context)
        {
            if (context.Item is Guid userId)
            {
                if (context.Action is MenuAction.ViewProfile)
                {
                    ShowUserProfile(userId);
                }
                else if (context.Action is MenuAction.SetLeader)
                {
                    TeamService.UpdateTeamLeader(CurrentTeam.Id, userId);
                }
                else if (context.Action is MenuAction.UnsetLeader)
                {
                    TeamService.UpdateTeamLeader(CurrentTeam.Id, null);
                }
            }
            else if (context.Item is TeamMember member)
            {
                if (context.Action is MenuAction.RemoveTeamMember)
                {
                    ModalService.ShowConfirmModal(
                        $"Вы действительно хотите исключить {member.FullName}?",
                        () => TeamService.KickMemberAsync(member),
                        confirmButtonVariant: ButtonVariant.Danger,
                        confirmButtonText: "Удалить"
                    );
                }
            }
        }
    }
}
