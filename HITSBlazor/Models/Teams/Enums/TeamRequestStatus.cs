using System.ComponentModel;

namespace HITSBlazor.Models.Teams.Enums
{
    public enum TeamRequestStatus
    {
        [Description("Новая")]
        New,

        [Description("Аннулирована")]
        Annulled,

        [Description("Принята")]
        Accepted,

        [Description("Отклонена")]
        Canceled,

        [Description("Отозвана")]
        Withdrawn
    }
}
