using HITSBlazor.Utils.Attributes;
using System.ComponentModel;

namespace HITSBlazor.Models.Users.Enums
{
    public enum RoleType
    {
        //=== bg-primary-subtle text-primary стили ===
        [Description("Инициатор")]
        [Style("bg-primary-subtle text-primary")]
        Initiator,

        [Description("Владелец команды")]
        [Style("bg-primary-subtle text-primary")]
        TeamOwner,

        [Description("Лидер команды")]
        [Style("bg-primary-subtle text-primary")]
        TeamLeader,

        [Description("Студент")]
        [Style("bg-primary-subtle text-primary")]
        Member,

        //=== bg-success-subtle text-success стили ===
        [Description("Проектный офис")]
        [Style("bg-success-subtle text-success")]
        ProjectOffice,

        [Description("Эксперт")]
        [Style("bg-success-subtle text-success")]
        Expert,

        [Description("Преподаватель")]
        [Style("bg-success-subtle text-success")]
        Teacher,

        //=== bg-danger-subtle text-danger стили ===
        [Description("Админ")]
        [Style("bg-danger-subtle text-danger")]
        Admin
    }
}
