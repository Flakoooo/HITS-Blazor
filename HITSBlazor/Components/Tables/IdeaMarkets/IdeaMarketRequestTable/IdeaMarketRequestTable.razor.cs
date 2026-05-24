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
using static HITSBlazor.Utils.Mocks.Common.MockInvitation;

namespace HITSBlazor.Components.Tables.IdeaMarkets.IdeaMarketRequestTable
{
    public partial class IdeaMarketRequestTable
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
        private readonly List<RequestTeamToIdea> _requestsTeamsToIdea = [];

        private static List<TableHeaderItem> RequestsTableHeader { get; } =
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

            TeamService.OnRequestTeamInIdeaStatusUpdated += RequestStatusHasUpdated;

            await LoadRequestsAsync();

            _isLoading = false;
            MarkAsInitialized();
        }

        protected override async Task AdditionalAfterRenderMethod()
        {
            if (_tableComponent != null)
                _tableContainer = _tableComponent.ScrollContainer;
        }

        protected override async Task OnLoadMoreItemsAsync() => await LoadRequestsAsync(true);

        protected override int GetCurrentItemsCount() => _requestsTeamsToIdea.Count;

        private async Task LoadRequestsAsync(bool append = false)
        {
            await LoadDataAsync(
                _requestsTeamsToIdea,
                () => TeamService.GetRequestsTeamToIdeasAsync(
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
            await LoadRequestsAsync();
        }

        private Dictionary<MenuAction, object> GetActions(RequestTeamToIdea request)
        {
            var actions = new Dictionary<MenuAction, object>
            {
                [MenuAction.ViewTeamProfile] = request.TeamId,
                [MenuAction.ViewLetter] = request.Letter
            };

            if (request.Status is TeamRequestStatus.New)
            {
                var currentUser = AuthService.CurrentUser;
                if (currentUser is not null)
                {
                    if (currentUser.Role is RoleType.Admin || currentUser.Id == CurrentIdeaMarket.Initiator.Id)
                    {
                        actions.Add(MenuAction.TeamRequestAccept, request.Id);
                        actions.Add(MenuAction.TeamRequestCancel, request.Id);
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
                else if (context.Action is MenuAction.TeamRequestAccept)
                {
                    TeamService.UpdateRequestTeamToIdeaStatusAsync(id, TeamRequestStatus.Accepted);
                }
                else if (context.Action is MenuAction.TeamRequestCancel)
                {
                    TeamService.UpdateRequestTeamToIdeaStatusAsync(id, TeamRequestStatus.Canceled);
                }
            }
            else if (context.Item is string letter)
            {
                if (context.Action is MenuAction.ViewLetter)
                {
                    ModalService.ShowLetterModal(false, letter: letter);
                }
            }
        }

        private async Task RequestStatusHasUpdated(Guid requestId, TeamRequestStatus newStatus)
        {
            var requestForUpdate = _requestsTeamsToIdea.FirstOrDefault(rtti => rtti.Id == requestId);
            if (requestForUpdate is null) return;

            if (newStatus is TeamRequestStatus.Accepted)
            {
                await LoadRequestsAsync();
            }
            else
            {
                requestForUpdate.Status = newStatus;
                StateHasChanged();
            }
        }

        protected override async ValueTask DisposeAsyncCore()
        {
            TeamService.OnRequestTeamInIdeaStatusUpdated -= RequestStatusHasUpdated;

            await ValueTask.CompletedTask;
        }
    }
}
