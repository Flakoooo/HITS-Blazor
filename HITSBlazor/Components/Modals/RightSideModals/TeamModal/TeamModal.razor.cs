using ApexCharts;
using HITSBlazor.Components.Tables.TableActionMenu;
using HITSBlazor.Components.Tables.TableHeader;
using HITSBlazor.Models.Common.Entities;
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
        private List<TeamInvitation> _teamInvitations = [];
        private List<RequestToTeam> _requestsToTeam = [];
        private List<RequestTeamToIdea> _requestsTeamToIdeas = [];
        private List<InvitationTeamToIdea> _invitationsTeamToIdeas = [];

        private TeamTableCategory _activeTableCategory = TeamTableCategory.Members;
        private TeamInfoCategory _activeInfoCategory = TeamInfoCategory.Info;

        private static IReadOnlyList<TableHeaderItem> MembersTableHeader { get; } =
        [
            new() { Text = "Почта", ColumnClass = "col-5" },
            new() { Text = "Имя", ColumnClass = "col-3" },
            new() { Text = "Фамилия", ColumnClass = "col-3" }
        ];

        private static IReadOnlyList<TableHeaderItem> NewMembersTableHeader { get; } =
        [
            new() { Text = "Статус", InCentered = true, ColumnClass = "col-1" },
            new() { Text = "Почта", InCentered = true, ColumnClass = "col-3" },
            new() { Text = "Имя", InCentered = true, ColumnClass = "col-3" },
            new() { Text = "Фамилия", InCentered = true, ColumnClass = "col-3" }
        ];

        private static IReadOnlyList<TableHeaderItem> RequestsToIdeasTableHeader { get; } =
        [
            new() { Text = "Название", ColumnClass = "col-7" },
            new() { Text = "Статус", InCentered = true, ColumnClass = "col-4" }
        ];
        private static IReadOnlyList<TableHeaderItem> InvitationsToIdeasTableHeader { get; } =
        [
            new() { Text = "Статус",InCentered = true },
            new() { Text = "Название", ColumnClass = "col-5" },
            new() { Text = "Компетенции", InCentered = true, ColumnClass = "col-4" }
        ];

        protected override async Task OnInitializedAsync()
        {
            _isLoading = true;

            _currentTeam = await TeamService.GetTeamByIdAsync(TeamId);
            if (_currentTeam is null) return;

            _teamInvitations = await TeamService.GetTeamInvitationsAsync(TeamId);
            _requestsToTeam = await TeamService.GetTeamRequestsToTeamAsync(TeamId);
            _requestsTeamToIdeas = await TeamService.GetRequestsTeamToIdeasAsync(TeamId);
            _requestsTeamToIdeas = [.. _requestsTeamToIdeas, .. _requestsTeamToIdeas, .. _requestsTeamToIdeas];
            _invitationsTeamToIdeas = await TeamService.GetInvitationsTeamToIdeasAsync(TeamId);

            _isLoading = false;
        }

        private string GetTableCategoryClass(TeamTableCategory category)
            => _activeTableCategory == category ? "active text-primary" : "text-secondary";

        private string GetInfoCategoryClass(TeamInfoCategory category)
            => _activeInfoCategory == category ? "btn-primary" : "btn-secondary";

        private void HandleTableMenuClick(TableActionContext context)
        {
            if (context.Action == TableAction.ViewProfile)
            {
                ModalService.ShowProfileModal((Guid)context.Item);
            }
            if (_activeTableCategory == TeamTableCategory.Members)
            {
                if (context.Action == TableAction.SetLeader)
                {
                    Console.WriteLine($"Назначение лидером {context.Item}");
                }
                else if (context.Action == TableAction.UnsetLeader)
                {
                    Console.WriteLine($"Снтие лидера {context.Item}");
                }
                else if (context.Action == TableAction.RemoveTeamMember)
                {
                    Console.WriteLine($"Исключение {context.Item}");
                }
            }
            else if (_activeTableCategory == TeamTableCategory.RequestsToTeam)
            {
                if (context.Action == TableAction.TeamRequestAccept)
                {

                }
                else if (context.Action == TableAction.TeamRequestCancel)
                {

                }
            }
        }

        private static ApexChartOptions<Skill> GetRadarChartOptions() => new()
        {
            Chart = new Chart
            {
                Type = ChartType.Radar,
                Toolbar = new Toolbar { Show = false }
            },
            Yaxis =
            [
                new YAxis
                {
                    Min = 0,
                    Max = 1,
                    Labels = new YAxisLabels { Show = false },
                    Show = false
                }
            ],
            Xaxis = new XAxis
            {
                Labels = new XAxisLabels
                {
                    Style = new AxisLabelStyle
                    {
                        Colors = new List<string> { "#a8a8a8" },
                        FontSize = "11px"
                    }
                }
            },
            Stroke = new Stroke { Width = 2 },
            Fill = new Fill { 
                Opacity = 0.5,
                Type = new List<FillType> { FillType.Solid }
            },
            Markers = new Markers { Size = 4 },
            Legend = new Legend
            {
                Show = true,
                Position = LegendPosition.Bottom
            }
        };

        internal class ShowTeamModal
        {
        }
    }
}
