using HITSBlazor.Models.Users.Enums;

namespace HITSBlazor.Utils.EnumUIConverters
{
    public static partial class EnumUIConverter
    {
        private static EnumUIResult GetRoleTypeInfo(RoleType type) => type switch
        {
            RoleType.Initiator => new EnumUIResult("Инициатор", "bg-primary-subtle text-primary"),
            RoleType.TeamOwner => new EnumUIResult("Владелец команды", "bg-primary-subtle text-primary"),
            RoleType.TeamLeader => new EnumUIResult("Лидер команды", "bg-primary-subtle text-primary"),
            RoleType.Member => new EnumUIResult("Студент", "bg-primary-subtle text-primary"),
            RoleType.ProjectOffice => new EnumUIResult("Проектный офис", "bg-success-subtle text-success"),
            RoleType.Expert => new EnumUIResult("Эксперт", "bg-success-subtle text-success"),
            RoleType.Teacher => new EnumUIResult("Преподаватель", "bg-success-subtle text-success"),
            RoleType.Admin => new EnumUIResult("Админ", "bg-danger-subtle text-danger"),
            _ => new EnumUIResult(type.ToString(), string.Empty)
        };
    }
}
