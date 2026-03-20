using HITSBlazor.Utils.Attributes;
using System.ComponentModel;

namespace HITSBlazor.Models.Teams.Enums
{
    public enum TeamRequestStatus
    {
        //=== bg-primary-subtle text-primary стили ===
        [Description("Новая")]
        [Style("bg-primary-subtle text-primary")]
        New,

        [Description("Аннулирована")]
        [Style("bg-primary-subtle text-primary")]
        Annulled,

        //=== bg-success-subtle text-success стили ===
        [Description("Принята")]
        [Style("bg-success-subtle text-success")]
        Accepted,

        //=== bg-danger-subtle text-danger стили ===
        [Description("Отклонена")]
        [Style("bg-danger-subtle text-danger")]
        Canceled,

        //=== bg-warning-subtle text-warning стили ===
        [Description("Отозвана")]
        [Style("bg-warning-subtle text-warning")]
        Withdrawn
    }
}
