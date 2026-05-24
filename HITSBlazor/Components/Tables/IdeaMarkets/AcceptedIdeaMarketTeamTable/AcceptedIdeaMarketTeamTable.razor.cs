using HITSBlazor.Components.ActionMenus.BaseActionMenu;
using HITSBlazor.Components.Tables.TableHeader;
using HITSBlazor.Models.Teams.Entities;
using HITSBlazor.Services.Modal;
using Microsoft.AspNetCore.Components;

namespace HITSBlazor.Components.Tables.IdeaMarkets.AcceptedIdeaMarketTeamTable
{
    public partial class AcceptedIdeaMarketTeamTable
    {
        [Inject]
        private ModalService ModalService { get; set; } = null!;

        [Parameter]
        public bool IsLoading { get; set; } = false;

        [Parameter]
        public Team? CurrentTeam { get; set; }

        private static List<TableHeaderItem> AcceptedTeamTableHeader { get; } =
        [
            new() { Text = "Название", ColumnClass = "col-3" },
            new() { Text = "Лидер", ColumnClass = "col-3" },
            new() { Text = "Участники", InCentered = true, OrderBy = nameof(Team.MembersCount) },
            new() { Text = "Компетенции", InCentered = true, ColumnClass = "col-4" }
        ];

        private void ShowTeamModal(Guid teamId) => ModalService.ShowTeamModal(teamId);

        private void ShowUserProfile(Guid userId) => ModalService.ShowProfileModal(userId);

        private void HandleTableMenuClick(TableActionContext context)
        {
            if (context.Action is MenuAction.ViewTeamProfile)
            {
                if (context.Item is Guid teamId)
                {
                    ShowTeamModal(teamId);
                }
            }
        }
    }
}
