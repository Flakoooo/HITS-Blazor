using HITSBlazor.Components.ActionMenus.BaseActionMenu;
using HITSBlazor.Components.Tables.TableHeader;
using HITSBlazor.Models.Markets.Entities;
using HITSBlazor.Models.Teams.Entities;
using HITSBlazor.Models.Teams.Enums;
using HITSBlazor.Models.Users.Enums;
using HITSBlazor.Services.Auth;
using HITSBlazor.Services.Modal;
using HITSBlazor.Services.Teams;
using Microsoft.AspNetCore.Components;

namespace HITSBlazor.Components.Tables.IdeaMarkets.IdeaMarketInvitedTeamsTable
{
    public partial class IdeaMarketInvitedTeamsTable
    {
        [Inject]
        private IAuthService AuthService { get; set; } = null!;

        [Inject]
        private ITeamService TeamService { get; set; } = null!;

        [Inject]
        private ModalService ModalService { get; set; } = null!;

        [Parameter]
        public required IdeaMarket CurrentIdeaMarket { get; set; }

        private bool _isLoading = true;
        private TableComponent.TableComponent? _tableComponent;

        private string _searchText = string.Empty;
        private readonly List<InvitationTeamToIdea> _invitationsTeamsToIdea = [];

        private static List<TableHeaderItem> InvitedTeamsTableHeader { get; } =
        [
            new() { Text = "" },
            new() { Text = "Название", ColumnClass = "col-3" },
            new() { Text = "Статус", ColumnClass = "col-3" },
            new() { Text = "Участники", InCentered = true, OrderBy = nameof(Team.MembersCount) },
            new() { Text = "Компетенции", InCentered = true, ColumnClass = "col-4" }
        ];

        protected override async Task OnInitializedAsync()
        {
            _isLoading = true;

            TeamService.OnInvitationTeamInIdeaStatusUpdated += InvitationStatusHasChanged;

            await LoadInvitationsAsync();

            _isLoading = false;
            MarkAsInitialized();
        }

        protected override async Task AdditionalAfterRenderMethod()
        {
            if (_tableComponent != null)
                _tableContainer = _tableComponent.ScrollContainer;
        }

        protected override async Task OnLoadMoreItemsAsync() => await LoadInvitationsAsync(true);

        protected override int GetCurrentItemsCount() => _invitationsTeamsToIdea.Count;

        private async Task LoadInvitationsAsync(bool append = false)
        {
            await LoadDataAsync(
                _invitationsTeamsToIdea,
                () => TeamService.GetInvitationsTeamToIdeasAsync(
                    _currentPage,
                    ideaMarketId: CurrentIdeaMarket.Id,
                    searchText: _searchText
                ),
                append
            );
        }

        private async Task SearchData(string value)
        {
            _searchText = value;
            ResetPagination();
            await LoadInvitationsAsync();
        }

        private Dictionary<MenuAction, object> GetActions(InvitationTeamToIdea invitation)
        {
            var actions = new Dictionary<MenuAction, object>
            {
                [MenuAction.ViewTeamProfile] = invitation.TeamId
            };

            if (invitation.Status is TeamRequestStatus.New)
            {
                var currentUser = AuthService.CurrentUser;
                if (currentUser is not null)
                {
                    if (currentUser.Role is RoleType.Admin || currentUser.Id == CurrentIdeaMarket.Initiator.Id)
                    {
                        actions.Add(MenuAction.TeamRequestWithdraw, invitation.Id);
                    }
                }
            }

            return actions;
        }

        private void ShowTeam(Guid teamId) => ModalService.ShowTeamModal(teamId);

        private void HandleTableMenuClick(TableActionContext context)
        {
            if (context.Item is Guid id)
            {
                if (context.Action is MenuAction.ViewIdeaMarket)
                {
                    ShowTeam(id);
                }
                else if (context.Action is MenuAction.TeamRequestWithdraw)
                {
                    TeamService.UpdateInvitationTeamToIdeaStatusAsync(id, TeamRequestStatus.Withdrawn);
                }
            }
        }

        private async Task InvitationStatusHasChanged(Guid invitationId, TeamRequestStatus newStatus)
        {
            var invitationForUpdate = _invitationsTeamsToIdea.FirstOrDefault(itti => itti.Id == invitationId);
            if (invitationForUpdate is null) return;

            invitationForUpdate.Status = newStatus;
            StateHasChanged();
        }

        protected override async ValueTask DisposeAsyncCore()
        {
            TeamService.OnInvitationTeamInIdeaStatusUpdated -= InvitationStatusHasChanged;

            await ValueTask.CompletedTask;
        }
    }
}
