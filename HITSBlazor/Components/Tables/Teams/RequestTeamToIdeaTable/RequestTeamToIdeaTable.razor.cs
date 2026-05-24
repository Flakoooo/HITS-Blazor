using HITSBlazor.Components.ActionMenus.BaseActionMenu;
using HITSBlazor.Components.Button;
using HITSBlazor.Components.Tables.TableHeader;
using HITSBlazor.Models.Teams.Entities;
using HITSBlazor.Models.Teams.Enums;
using HITSBlazor.Models.Users.Enums;
using HITSBlazor.Services.Auth;
using HITSBlazor.Services.Modal;
using HITSBlazor.Services.Teams;
using Microsoft.AspNetCore.Components;

namespace HITSBlazor.Components.Tables.Teams.RequestTeamToIdeaTable
{
    public partial class RequestTeamToIdeaTable
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
        private readonly List<RequestTeamToIdea> _requestsTeamToIdeas = [];

        private static List<TableHeaderItem> RequestsToIdeasTableHeader { get; } =
        [
            new() { Text = "Название",  ColumnClass = "col-7"                       },
            new() { Text = "Статус",    ColumnClass = "col-4", InCentered = true    }
        ];

        protected override async Task OnInitializedAsync()
        {
            _isLoading = true;

            TeamService.OnRequestTeamInIdeaStatusUpdated += RequestStatusHasChanged;

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

        protected override int GetCurrentItemsCount() => _requestsTeamToIdeas.Count;

        private async Task LoadRequestsAsync(bool append = false)
        {
            await LoadDataAsync(
                _requestsTeamToIdeas,
                () => TeamService.GetRequestsTeamToIdeasAsync(
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
            await LoadRequestsAsync();
        }

        private Dictionary<MenuAction, object> GetActions(RequestTeamToIdea request)
        {
            var actions = new Dictionary<MenuAction, object>
            {
                [MenuAction.ViewIdeaMarket] = request.IdeaMarketId,
                [MenuAction.ViewLetter] = request.Letter
            };

            if (request.Status is TeamRequestStatus.New)
            {
                var currentUser = AuthService.CurrentUser;
                if (currentUser is not null)
                {
                    if (currentUser.Role is RoleType.Admin || currentUser.Id == CurrentTeam.Owner.UserId || currentUser.Id == CurrentTeam.Leader?.UserId)
                    {
                        actions.Add(MenuAction.TeamRequestWithdraw, request.Id);
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
                else if (context.Action is MenuAction.TeamRequestWithdraw)
                {
                    ModalService.ShowConfirmModal(
                        $"Вы действительно хотите отозвать заявку?",
                        () => TeamService.UpdateRequestTeamToIdeaStatusAsync(id, TeamRequestStatus.Withdrawn),
                        confirmButtonVariant: ButtonVariant.Danger,
                        confirmButtonText: "Отозвать"
                    );
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

        private async Task RequestStatusHasChanged(Guid requestId, TeamRequestStatus newStatus)
        {
            var invitationForUpdate = _requestsTeamToIdeas.FirstOrDefault(ti => ti.Id == requestId);
            if (invitationForUpdate is null) return;

            invitationForUpdate.Status = newStatus;
            StateHasChanged();
        }

        protected override async ValueTask DisposeAsyncCore()
        {
            TeamService.OnRequestTeamInIdeaStatusUpdated -= RequestStatusHasChanged;

            await ValueTask.CompletedTask;
        }
    }
}
