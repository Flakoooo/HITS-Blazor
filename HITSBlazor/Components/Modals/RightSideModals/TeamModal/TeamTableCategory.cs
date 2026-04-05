using System.ComponentModel;

namespace HITSBlazor.Components.Modals.RightSideModals.TeamModal
{
    public enum TeamTableCategory
    {
        [Description("Участники")]
        Members,

        [Description("Приглашения в команду")]
        Invitations,

        [Description("Заявки в команду")]
        RequestsToTeam,

        [Description("Заявки в идеи")]
        RequestsTeamToIdeas,

        [Description("Приглашения в идеи")]
        InvitationsTeamToIdeas,
    }
}
