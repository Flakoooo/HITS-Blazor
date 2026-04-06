using HITSBlazor.Components.ActionMenus.BaseActionMenu;
using HITSBlazor.Components.Modals.Components;
using HITSBlazor.Components.Modals.Components.RightSideModaCollapselInfo;
using HITSBlazor.Components.Modals.Components.RightSideModalInfo;
using HITSBlazor.Components.Modals.RightSideModals.IdeaModal;
using HITSBlazor.Components.Tables.TableHeader;
using HITSBlazor.Models.Common.Entities;
using HITSBlazor.Models.Ideas.Entities;
using HITSBlazor.Models.Teams.Entities;
using HITSBlazor.Services.Modal;
using HITSBlazor.Services.Teams;
using Microsoft.AspNetCore.Components;

namespace HITSBlazor.Components.Modals.RightSideModals.TeamModal
{
    public partial class TeamModal
    {
        [Inject]
        private ITeamService TeamService { get; set; } = null!;

        [Inject]
        private ModalService ModalService { get; set; } = null!;

        [Parameter]
        public Guid TeamId { get; set; }

        private bool _isLoading = true;

        private Team? _currentTeam;

        private List<CollapseItem> _teamData = [];

        private List<TeamInvitation> _teamInvitations = [];
        private List<RequestToTeam> _requestsToTeam = [];
        private List<RequestTeamToIdea> _requestsTeamToIdeas = [];
        private List<InvitationTeamToIdea> _invitationsTeamToIdeas = [];

        private TeamTableCategory _activeTableCategory = TeamTableCategory.Members;

        private static List<TableHeaderItem> MembersTableHeader { get; } =
        [
            new() { Text = "Почта", ColumnClass = "col-5" },
            new() { Text = "Имя", ColumnClass = "col-3" },
            new() { Text = "Фамилия", ColumnClass = "col-3" }
        ];

        private static List<TableHeaderItem> NewMembersTableHeader { get; } =
        [
            new() { Text = "Статус", InCentered = true, ColumnClass = "col-1" },
            new() { Text = "Почта", InCentered = true, ColumnClass = "col-3" },
            new() { Text = "Имя", InCentered = true, ColumnClass = "col-3" },
            new() { Text = "Фамилия", InCentered = true, ColumnClass = "col-3" }
        ];

        private static List<TableHeaderItem> RequestsToIdeasTableHeader { get; } =
        [
            new() { Text = "Название", ColumnClass = "col-7" },
            new() { Text = "Статус", InCentered = true, ColumnClass = "col-4" }
        ];
        private static List<TableHeaderItem> InvitationsToIdeasTableHeader { get; } =
        [
            new() { Text = "Статус",InCentered = true },
            new() { Text = "Название", ColumnClass = "col-5" },
            new() { Text = "Компетенции", InCentered = true, ColumnClass = "col-4" }
        ];

        private readonly List<RightSideModalInfoItem> _infoItems =
        [
            new RightSideModalInfoItem
            {
                Label = "Владелец команды",
                Icon = "bi-person-circle",
                IsLinkable = true
            },
            new RightSideModalInfoItem
            {
                Label = "Тим-лидер команды",
                Icon = "bi-person-circle"
            },
            new RightSideModalInfoItem
            {
                Label = "Дата создания",
                Icon = "bi-calendar-date",
                TextColor = Typography.TextColor.Primary
            },
            new RightSideModalInfoItem
            {
                IsInline = true,
                Label = "Количество участников:",
                TextColor = Typography.TextColor.Primary
            }
        ];

        protected override async Task OnInitializedAsync()
        {
            _isLoading = true;

            _currentTeam = await TeamService.GetTeamByIdAsync(TeamId);
            if (_currentTeam is null) return;

            _teamData = GetTeamData();

            _teamInvitations = await TeamService.GetTeamInvitationsAsync(TeamId);
            _requestsToTeam = await TeamService.GetTeamRequestsToTeamAsync(TeamId);
            _requestsTeamToIdeas = await TeamService.GetRequestsTeamToIdeasAsync(TeamId);
            _invitationsTeamToIdeas = await TeamService.GetInvitationsTeamToIdeasAsync(TeamId);

            var ownerInfoItem = _infoItems[0];
            ownerInfoItem.Text = _currentTeam.Owner.FullName;
            ownerInfoItem.LinkMethod = () => ModalService.ShowProfileModal(_currentTeam.Owner.UserId);

            var leaderInfoItem = _infoItems[1];
            leaderInfoItem.Text = _currentTeam.Leader?.FullName ?? "-";
            if (_currentTeam.Leader is not null)
            {
                leaderInfoItem.IsLinkable = true;
                leaderInfoItem.LinkMethod = () => ModalService.ShowProfileModal(_currentTeam.Leader.UserId);
            }

            _infoItems[2].Text = _currentTeam.CreatedAt.ToString("dd.MM.yyyy");

            _infoItems[3].Text = _currentTeam.MembersCount.ToString();

            _isLoading = false;
        }

        private List<CollapseItem> GetTeamData() => [
            new() { Title = "Описание команды", Data = _currentTeam?.Description },
        ];

        private string GetTableCategoryClass(TeamTableCategory category)
            => _activeTableCategory == category ? "active text-primary" : "text-secondary";

        private void HandleTableMenuClick(TableActionContext context)
        {
            if (context.Action == MenuAction.ViewProfile)
            {
                ModalService.ShowProfileModal((Guid)context.Item);
            }
            if (_activeTableCategory == TeamTableCategory.Members)
            {
                if (context.Action == MenuAction.SetLeader)
                {
                    Console.WriteLine($"Назначение лидером {context.Item}");
                }
                else if (context.Action == MenuAction.UnsetLeader)
                {
                    Console.WriteLine($"Снтие лидера {context.Item}");
                }
                else if (context.Action == MenuAction.RemoveTeamMember)
                {
                    Console.WriteLine($"Исключение {context.Item}");
                }
            }
            else if (_activeTableCategory == TeamTableCategory.RequestsToTeam)
            {
                if (context.Action == MenuAction.TeamRequestAccept)
                {

                }
                else if (context.Action == MenuAction.TeamRequestCancel)
                {

                }
            }
        }
    }
}
