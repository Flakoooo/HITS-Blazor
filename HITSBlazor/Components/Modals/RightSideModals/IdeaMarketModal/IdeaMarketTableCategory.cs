using System.ComponentModel;

namespace HITSBlazor.Components.Modals.RightSideModals.IdeaMarketModal
{
    public enum IdeaMarketTableCategory
    {
        [Description("Принятая команда")]
        AcceptedTeam,

        [Description("Заявки")]
        Requests,

        [Description("Приглашенные команды")]
        InvitedTeams
    }
}
