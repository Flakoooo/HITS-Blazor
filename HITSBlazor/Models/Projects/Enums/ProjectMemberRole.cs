using HITSBlazor.Utils.Attributes;
using System.ComponentModel;

namespace HITSBlazor.Models.Projects.Enums
{
    public enum ProjectMemberRole
    {
        [Description("Инициатор")]
        [Style("bg-success-subtle text-success")]
        Initiator,

        [Description("Тим-лидер")]
        [Style("bg-warning-subtle text-warning")]
        TeamLeader,

        [Description("Участник")]
        [Style("bg-primary-subtle text-primary")]
        Member,
    }
}
