using HITSBlazor.Components.ActionMenus.BaseActionMenu;
using HITSBlazor.Components.Tables.TableHeader;
using HITSBlazor.Models.Markets.Entities;
using HITSBlazor.Models.Teams.Entities;
using HITSBlazor.Services.Auth;
using HITSBlazor.Services.IdeaMarkets;
using HITSBlazor.Services.Modal;
using Microsoft.AspNetCore.Components;

namespace HITSBlazor.Components.Tables.IdeaMarkets.IdeaMarketInvitedTeamsTable
{
    public partial class IdeaMarketInvitedTeamsTable
    {
        [Inject]
        private IIdeaMarketService IdeaMarketService { get; set; } = null!;

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

        protected override int GetCurrentItemsCount() => _invitationsTeamsToIdea.Count;

        private async Task LoadRequestsAsync(bool append = false)
        {
            await LoadDataAsync(
                _invitationsTeamsToIdea,
                () => IdeaMarketService.GetInvitationTeamsToIdeaAsync(
                    _currentPage,
                    ideaId: CurrentIdeaMarket.IdeaId,
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

        private static Dictionary<MenuAction, object> GetActions(InvitationTeamToIdea invitation)
        {
            var actions = new Dictionary<MenuAction, object>
            {
                [MenuAction.ViewTeamProfile] = invitation.TeamId
            };

            return actions;
        }

        private void ShowTeam(Guid teamId) => ModalService.ShowTeamModal(teamId);

        private void HandleTableMenuClick(TableActionContext context)
        {
            if (context.Item is Guid teamId)
            {
                if (context.Action is MenuAction.ViewIdeaMarket)
                {
                    ShowTeam(teamId);
                }
            }
        }
    }
}
