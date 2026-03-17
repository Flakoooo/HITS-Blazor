using System.ComponentModel;

namespace HITSBlazor.Models.Users.Enums
{
    public enum RoleType
    {
        [Description("Инициатор")]
        Initiator,

        [Description("Владелец команды")]
        TeamOwner,

        [Description("Лидер команды")]
        TeamLeader,

        [Description("Студент")]
        Member,

        [Description("Проектный офис")]
        ProjectOffice,

        [Description("Эксперт")]
        Expert,

        [Description("Админ")]
        Admin,

        [Description("Преподаватель")]
        Teacher
    }
}
