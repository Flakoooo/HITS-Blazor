using HITSBlazor.Models.Teams.Enums;

namespace HITSBlazor.Utils.EnumUIConverters
{
    public static partial class EnumUIConverter
    {
        private static EnumUIResult GetTeamRequestInfo(TeamRequestStatus status) => status switch
        {
            TeamRequestStatus.New => new EnumUIResult("Новая", "bg-primary-subtle text-primary"),
            TeamRequestStatus.Annulled => new EnumUIResult("Аннулирована", "bg-primary-subtle text-primary"),
            TeamRequestStatus.Accepted => new EnumUIResult("Принята", "bg-success-subtle text-success"),
            TeamRequestStatus.Canceled => new EnumUIResult("Отклонена", "bg-danger-subtle text-danger"),
            TeamRequestStatus.Withdrawn => new EnumUIResult("Отозвана", "bg-warning-subtle text-warning"),
            _ => new EnumUIResult(status.ToString(), string.Empty)
        };
    }
}
