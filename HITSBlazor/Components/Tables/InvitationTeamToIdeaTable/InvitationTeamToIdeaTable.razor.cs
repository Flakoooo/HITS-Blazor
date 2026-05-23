using HITSBlazor.Components.ActionMenus.BaseActionMenu;
using HITSBlazor.Components.Tables.TableHeader;
using HITSBlazor.Models.Teams.Entities;
using HITSBlazor.Services.Auth;
using HITSBlazor.Services.Modal;
using HITSBlazor.Services.Teams;
using Microsoft.AspNetCore.Components;

namespace HITSBlazor.Components.Tables.InvitationTeamToIdeaTable
{
    public partial class InvitationTeamToIdeaTable
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

        protected override int GetCurrentItemsCount() => _invitationsTeamToIdeas.Count;

        private async Task LoadTeamMembersAsync(bool append = false)
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
            await LoadTeamMembersAsync();
        }

        private static Dictionary<MenuAction, object> GetActions(InvitationTeamToIdea invitation)
        {
            var actions = new Dictionary<MenuAction, object>
            {
                [MenuAction.ViewIdea] = invitation.IdeaId
            };

            return actions;
        }

        private void ShowIdea(Guid ideaId) => ModalService.ShowIdeaModal(ideaId);

        private void HandleTableMenuClick(TableActionContext context)
        {
            if (context.Item is Guid ideaId)
            {
                if (context.Action is MenuAction.ViewIdeaMarket)
                {
                    ShowIdea(ideaId);
                }
            }
        }
    }
}
