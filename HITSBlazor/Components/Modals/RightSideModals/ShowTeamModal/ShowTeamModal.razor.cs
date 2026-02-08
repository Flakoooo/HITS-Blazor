
using HITSBlazor.Components.Tables.TableActionMenu;
using HITSBlazor.Components.Tables.TableHeader;
using HITSBlazor.Models.Teams.Entities;
using HITSBlazor.Services.Modal;
using HITSBlazor.Services.Teams;
using Microsoft.AspNetCore.Components;

namespace HITSBlazor.Components.Modals.RightSideModals.ShowTeamModal
{
    public partial class ShowTeamModal
    {
        [Inject]
        private ITeamService TeamService { get; set; } = null!;

        [Inject]
        private ModalService ModalService { get; set; } = null!;

        [Parameter]
        public Guid TeamId { get; set; }

        private bool _isLoading = true;

        private Team? _currentTeam;
        private List<TeamInvitation> _teamInvitations = [];
        private List<RequestToTeam> _requestsToTeam = [];
        private List<RequestTeamToIdea> _requestsTeamToIdeas = [];
        private List<InvitationTeamToIdea> _invitationsTeamToIdeas = [];

        // сделать enum для раделения логики
        private TeamTableCategory _activeCategory = TeamTableCategory.Members;

        protected override async Task OnInitializedAsync()
        {
            _isLoading = true;

            _currentTeam = await TeamService.GetTeamByIdAsync(TeamId);
            if (_currentTeam is null) return;

            _teamInvitations = await TeamService.GetTeamInvitationsAsync(TeamId);
            _requestsToTeam = await TeamService.GetTeamRequestsToTeamAsync(TeamId);
            _requestsTeamToIdeas = await TeamService.GetRequestsTeamToIdeasAsync(TeamId);
            _invitationsTeamToIdeas = await TeamService.GetInvitationsTeamToIdeasAsync(TeamId);

            _isLoading = false;
        }

        private string GetCategoryClass(TeamTableCategory category)
            => _activeCategory == category ? "active text-primary" : "text-secondary";

        private static List<TableHeaderItem> GetMembersTableHeader() =>
        [
            new TableHeaderItem
            {
                Text = "Почта",
                ColumnClass = "col-5"
            },
            new TableHeaderItem
            {
                Text = "Имя",
                ColumnClass = "col-3"
            },
            new TableHeaderItem
            {
                Text = "Фамилия",
                ColumnClass = "col-3"
            }
        ];

        private static List<TableHeaderItem> GetNewMembersTableHeader() =>
        [
            new TableHeaderItem
            {
                Text = "Статус",
                InCentered = true,
                ColumnClass = "col-1"
            },
            new TableHeaderItem
            {
                Text = "Почта",
                InCentered = true,
                ColumnClass = "col-3"
            },
            new TableHeaderItem
            {
                Text = "Имя",
                InCentered = true,
                ColumnClass = "col-3"
            },
            new TableHeaderItem
            {
                Text = "Фамилия",
                InCentered = true,
                ColumnClass = "col-3"
            }
        ];

        private void HandleTableMenuClick(TableActionContext context)
        {
            if (context.Action == TableAction.ViewProfile)
            {
                var parameters = new Dictionary<string, object>
                {
                    { "UserId", context.ItemId }
                };
                ModalService.Show<ShowUserModal.ShowUserModal>(ModalType.RightSide, parameters: parameters);
            }
            else if (_activeCategory == TeamTableCategory.RequestsToTeam)
            {
                if (context.Action == TableAction.TeamRequestAccept)
                {

                }
                else if (context.Action == TableAction.TeamRequestCancel)
                {

                }
            }
        }
    }
}
