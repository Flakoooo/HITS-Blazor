using HITSBlazor.Components.Modals.RightSideModals.TeamModal;

namespace HITSBlazor.Utils.EnumUIConverters
{
    public static partial class EnumUIConverter
    {
        private static EnumUIResult GetTeamCategoryInfo(TeamTableCategory status) => status switch
        {
            TeamTableCategory.Members => new EnumUIResult("Участники", string.Empty),
            TeamTableCategory.Invitations => new EnumUIResult("Приглашения в команду", string.Empty),
            TeamTableCategory.RequestsToTeam => new EnumUIResult("Заявки в команду", string.Empty),
            TeamTableCategory.RequestsTeamToIdeas => new EnumUIResult("Заявки в идеи", string.Empty),
            TeamTableCategory.InvitationsTeamToIdeas => new EnumUIResult("Приглашения в идеи", string.Empty),
            _ => new EnumUIResult(status.ToString(), string.Empty)
        };
    }
}
