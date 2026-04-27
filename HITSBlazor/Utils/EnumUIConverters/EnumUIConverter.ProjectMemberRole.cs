using HITSBlazor.Models.Projects.Enums;
using HITSBlazor.Pages.Projects.ProjectView;

namespace HITSBlazor.Utils.EnumUIConverters
{
    public static partial class EnumUIConverter
    {
        private static EnumUIResult GetProjectMemberRoleInfo(ProjectMemberRole role) => role switch
        {
            ProjectMemberRole.Initiator => new EnumUIResult("Инициатор", "bg-success-subtle text-success"),
            ProjectMemberRole.TeamLeader => new EnumUIResult("Тим-лидер", "bg-warning-subtle text-warning"),
            ProjectMemberRole.Member => new EnumUIResult("Участник", "bg-primary-subtle text-primary"),
            _ => new EnumUIResult(role.ToString(), string.Empty)
        };
    }
}
