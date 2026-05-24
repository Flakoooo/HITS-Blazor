using HITSBlazor.Components.ActionMenus.BaseActionMenu;
using HITSBlazor.Components.Tables.TableHeader;
using HITSBlazor.Models.Markets.Entities;
using HITSBlazor.Models.Teams.Entities;
using HITSBlazor.Services.Auth;
using HITSBlazor.Services.IdeaMarkets;
using HITSBlazor.Services.Modal;
using Microsoft.AspNetCore.Components;

namespace HITSBlazor.Components.Tables.IdeaMarkets.IdeaMarketRequestTable
{
    public partial class IdeaMarketRequestTable
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
                () => IdeaMarketService.GetRequestsTeamToIdeaAsync(
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

        private static Dictionary<MenuAction, object> GetActions(RequestTeamToIdea request)
        {
            var actions = new Dictionary<MenuAction, object>
            {
                [MenuAction.ViewTeamProfile] = request.TeamId,
                [MenuAction.ViewLetter] = request.Letter
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
            else if (context.Item is string letter)
            {
                if (context.Action is MenuAction.ViewLetter)
                {
                    ModalService.ShowLetterModal(false, letter: letter);
                }
            }
        }
    }
}
