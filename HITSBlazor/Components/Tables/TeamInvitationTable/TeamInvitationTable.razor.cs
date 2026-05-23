using HITSBlazor.Components.ActionMenus.BaseActionMenu;
using HITSBlazor.Components.Tables.TableHeader;
using HITSBlazor.Models.Quests.Entities;
using HITSBlazor.Models.Teams.Entities;
using HITSBlazor.Models.Teams.Enums;
using HITSBlazor.Models.Users.Enums;
using HITSBlazor.Services.Auth;
using HITSBlazor.Services.Modal;
using HITSBlazor.Services.Teams;
using Microsoft.AspNetCore.Components;

namespace HITSBlazor.Components.Tables.TeamInvitationTable
{
    public partial class TeamInvitationTable
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
        private readonly List<TeamInvitation> _teamInvitations = [];

        private static List<TableHeaderItem> NewMembersTableHeader { get; } =
        [
            new() { Text = "Статус", InCentered = true, ColumnClass = "col-1" },
            new() { Text = "Почта", InCentered = true, ColumnClass = "col-3" },
            new() { Text = "Имя", InCentered = true, ColumnClass = "col-3" },
            new() { Text = "Фамилия", InCentered = true, ColumnClass = "col-3" }
        ];

        protected override async Task OnInitializedAsync()
        {
            _isLoading = true;

            TeamService.OnNewTeamApplicationsHasCreated += LoadTeamMembersAsync;
            TeamService.OnTeamInvitationStatusHasChanged += InvitationStatusHasChanged;

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

        protected override int GetCurrentItemsCount() => _teamInvitations.Count;

        private async Task LoadTeamMembersAsync(bool append = false)
        {
            await LoadDataAsync(
                _teamInvitations,
                () => TeamService.GetTeamInvitationsAsync(
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

        private Dictionary<MenuAction, object> GetActions(TeamInvitation invitation)
        {
            var actions = new Dictionary<MenuAction, object>
            {
                [MenuAction.ViewProfile] = invitation.UserId
            };

            var currentUser = AuthService.CurrentUser;
            if (currentUser is not null)
            {
                if (invitation.Status is TeamRequestStatus.New)
                {
                    if (currentUser.Role is RoleType.Admin || currentUser.Id == CurrentTeam.Owner.Id || currentUser.Id == CurrentTeam.Leader?.Id)
                    {
                        actions.Add(MenuAction.TeamRequestWithdraw, invitation.Id);
                    }
                    else if (currentUser.Role is RoleType.Admin || invitation.UserId == currentUser.Id)
                    {
                        actions.Add(MenuAction.TeamRequestAccept, invitation.Id);
                        actions.Add(MenuAction.TeamRequestCancel, invitation.Id);
                    }
                }
            }

            return actions;
        }

        private void ShowUserProfile(Guid userId) => ModalService.ShowProfileModal(userId);

        private void HandleTableMenuClick(TableActionContext context)
        {
            if (context.Item is Guid id)
            {
                if (context.Action is MenuAction.ViewProfile)
                {
                    ShowUserProfile(id);
                }
                else if (context.Action is MenuAction.TeamRequestWithdraw)
                {
                    TeamService.UpdateTeamInvitationStatusAsync(id, TeamRequestStatus.Withdrawn);
                }
                else if (context.Action is MenuAction.TeamRequestAccept)
                {
                    TeamService.UpdateTeamInvitationStatusAsync(id, TeamRequestStatus.Accepted);
                }
                else if (context.Action is MenuAction.TeamRequestCancel)
                {
                    TeamService.UpdateTeamInvitationStatusAsync(id, TeamRequestStatus.Canceled);
                }
            }
        }

        private void InvitationStatusHasChanged(Guid invitationId, TeamRequestStatus newStatus)
        {
            var invitationForUpdate = _teamInvitations.FirstOrDefault(ti => ti.Id == invitationId);
            if (invitationForUpdate is null) return;

            invitationForUpdate.Status = newStatus;
            StateHasChanged();
        }

        protected override async ValueTask DisposeAsyncCore()
        {
            TeamService.OnNewTeamApplicationsHasCreated -= LoadTeamMembersAsync;
            TeamService.OnTeamInvitationStatusHasChanged -= InvitationStatusHasChanged;

            await ValueTask.CompletedTask;
        }
    }
}
