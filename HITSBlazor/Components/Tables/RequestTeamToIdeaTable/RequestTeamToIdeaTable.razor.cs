using HITSBlazor.Components.ActionMenus.BaseActionMenu;
using HITSBlazor.Components.Tables.TableHeader;
using HITSBlazor.Models.Teams.Entities;
using HITSBlazor.Services.Auth;
using HITSBlazor.Services.Modal;
using HITSBlazor.Services.Teams;
using Microsoft.AspNetCore.Components;

namespace HITSBlazor.Components.Tables.RequestTeamToIdeaTable
{
    public partial class RequestTeamToIdeaTable
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
        private readonly List<RequestTeamToIdea> _requestsTeamToIdeas = [];

        private static List<TableHeaderItem> RequestsToIdeasTableHeader { get; } =
        [
            new() { Text = "Название",  ColumnClass = "col-7"                       },
            new() { Text = "Статус",    ColumnClass = "col-4", InCentered = true    }
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

        protected override int GetCurrentItemsCount() => _requestsTeamToIdeas.Count;

        private async Task LoadTeamMembersAsync(bool append = false)
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
            await LoadTeamMembersAsync();
        }

        private static Dictionary<MenuAction, object> GetActions(RequestTeamToIdea request)
        {
            var actions = new Dictionary<MenuAction, object>
            {
                [MenuAction.ViewIdeaMarket] = request.IdeaMarketId,
                [MenuAction.ViewLetter] = request.Letter
            };

            return actions;
        }

        private void ShowIdeaMarket(Guid ideaMarketId) => ModalService.ShowIdeaMarketModal(ideaMarketId);

        private void HandleTableMenuClick(TableActionContext context)
        {
            if (context.Item is Guid ideaMarketId)
            {
                if (context.Action is MenuAction.ViewIdeaMarket)
                {
                    ShowIdeaMarket(ideaMarketId);
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
