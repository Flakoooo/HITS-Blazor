using HITSBlazor.Components.ActionMenus.BaseActionMenu;
using HITSBlazor.Components.Tables.TableHeader;
using HITSBlazor.Models.Teams.Entities;
using HITSBlazor.Models.Teams.Enums;
using HITSBlazor.Models.Users.Enums;
using HITSBlazor.Services.Auth;
using HITSBlazor.Services.Modal;
using HITSBlazor.Services.Teams;
using Microsoft.AspNetCore.Components;

namespace HITSBlazor.Components.Tables.Teams.InvitationTeamToIdeaTable
{
    public partial class InvitationTeamToIdeaTable
    {
        [Inject]
        private IAuthService AuthService { get; set; } = null!;

        [Inject]
        private ITeamService TeamService { get; set; } = null!;

        [Inject]
        private ModalService ModalService { get; set; } = null!;

        [Parameter]
        public required Team CurrentTeam { get; set; }

        private bool _isLoading = true;
        private TableComponent.TableComponent? _tableComponent;

        private string _searchText = string.Empty;
        private readonly List<InvitationTeamToIdea> _invitationsTeamToIdeas = [];

        private static List<TableHeaderItem> InvitationsToIdeasTableHeader { get; } =
        [
            new() { Text = "Статус",        InCentered = true },
            new() { Text = "Название",      ColumnClass = "col-5" },
            new() { Text = "Компетенции",   ColumnClass = "col-4", InCentered = true }
        ];

        protected override async Task OnInitializedAsync()
        {
            _isLoading = true;

            TeamService.OnNewInvitationTeamInIdeaCreated += LoadInvitationsAsync;
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

        protected override int GetCurrentItemsCount() => _invitationsTeamToIdeas.Count;

        private async Task LoadInvitationsAsync(bool append = false)
        {
            await LoadDataAsync(
                _invitationsTeamToIdeas,
                () => TeamService.GetInvitationsTeamToIdeasAsync(
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
            await LoadInvitationsAsync();
        }

        private Dictionary<MenuAction, object> GetActions(InvitationTeamToIdea invitation)
        {
            var actions = new Dictionary<MenuAction, object>
            {
                [MenuAction.ViewIdea] = invitation.IdeaId
            };

            if (invitation.Status is TeamRequestStatus.New)
            {
                var currentUser = AuthService.CurrentUser;
                if (currentUser is not null)
                {
                    if (currentUser.Role is RoleType.Admin || currentUser.Id == CurrentTeam.Owner.UserId || currentUser.Id == CurrentTeam.Leader?.UserId)
                    {
                        actions.Add(MenuAction.TeamRequestAccept, invitation.Id);
                        actions.Add(MenuAction.TeamRequestCancel, invitation.Id);
                    }
                }
            }

            return actions;
        }

        private void ShowIdeaMarket(Guid ideaMarketId) => ModalService.ShowIdeaMarketModal(ideaMarketId);

        private async Task HandleTableMenuClick(TableActionContext context)
        {
            if (context.Item is Guid id)
            {
                if (context.Action is MenuAction.ViewIdeaMarket)
                {
                    ShowIdeaMarket(id);
                }
                else if (context.Action is MenuAction.TeamRequestAccept)
                {
                    await TeamService.UpdateInvitationTeamToIdeaStatusAsync(id, TeamRequestStatus.Accepted);
                }
                else if (context.Action is MenuAction.TeamRequestCancel)
                {
                    await TeamService.UpdateInvitationTeamToIdeaStatusAsync(id, TeamRequestStatus.Canceled);
                }
            }
        }

        private async Task InvitationStatusHasChanged(Guid invitationId, TeamRequestStatus newStatus)
        {
            if (newStatus is TeamRequestStatus.Accepted)
            {
                await LoadInvitationsAsync();
            }
            else if (newStatus is TeamRequestStatus.Canceled)
            {
                var invitationForUpdate = _invitationsTeamToIdeas.FirstOrDefault(ti => ti.Id == invitationId);
                if (invitationForUpdate is null) return;

                invitationForUpdate.Status = newStatus;
            }
            StateHasChanged();
        }

        protected override async ValueTask DisposeAsyncCore()
        {
            TeamService.OnNewInvitationTeamInIdeaCreated -= LoadInvitationsAsync;
            TeamService.OnInvitationTeamInIdeaStatusUpdated -= InvitationStatusHasChanged;

            await ValueTask.CompletedTask;
        }
    }
}
