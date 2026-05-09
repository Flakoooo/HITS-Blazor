using HITSBlazor.Utils.Attributes;
using System.ComponentModel;

namespace HITSBlazor.Models.Projects.Enums
{
    public enum ProjectMemberRole
    {
        [Description("Участник")]
        [Style("bg-primary-subtle text-primary")]
        Member = 0,

        [Description("Тим-лидер")]
        [Style("bg-warning-subtle text-warning")]
        TeamLeader = 1,

        [Description("Инициатор")]
        [Style("bg-success-subtle text-success")]
        Initiator = 2
    }
}
