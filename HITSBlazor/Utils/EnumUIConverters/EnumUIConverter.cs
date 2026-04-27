using HITSBlazor.Components.ActionMenus.BaseActionMenu;
using HITSBlazor.Components.GlobalNotification;
using HITSBlazor.Components.Modals.CenterModals.EndedSprintModal;
using HITSBlazor.Components.Modals.RightSideModals.ProfileModal;
using HITSBlazor.Models.Common.Enums;
using HITSBlazor.Models.Ideas.Enums;
using HITSBlazor.Models.Markets.Enums;
using HITSBlazor.Models.Projects.Enums;
using HITSBlazor.Models.Users.Enums;
using HITSBlazor.Pages.Projects.ProjectView;
using HITSTaskStatus = HITSBlazor.Models.Projects.Enums.TaskStatus;

namespace HITSBlazor.Utils.EnumUIConverters
{
    public static partial class EnumUIConverter
    {
        public static EnumUIResult GetInfo<T>(T value) where T : struct, Enum => value switch
        {
            RoleType t => GetRoleTypeInfo(t),
            SkillType t => GetSkillTypeInfo(t),
            IdeaStatusType s => GetIdeasStatusInfo(s),
            IdeaMarketStatusType s => GetMarketIdeaStatusInfo(s),
            ProjectStatus s => GetProjectStatusInfo(s),
            HITSTaskStatus s => GetTastStatusInfo(s),
            ProjectMemberRole r => GetProjectMemberRoleInfo(r),
            SprintStatus s => GetSprintStatusInfo(s),
            ProfileModalCategory c => GetProfileCategoryInfo(c),
            ProjectViewCategory c => GetProjectCategoryInfo(c),
            EndedSprintModalStatCategory c => GetSprintModalStatCategoryInfo(c),
            MenuAction a => GetActionMenuInfo(a),
            NotificationType n => GetNotificationTypeInfo(n),
            _ => new EnumUIResult(value.ToString(), string.Empty)
        };

        public static EnumUIResult GetInfo(Enum? value) => value switch
        {
            RoleType t => GetRoleTypeInfo(t),
            SkillType t => GetSkillTypeInfo(t),
            IdeaStatusType s => GetIdeasStatusInfo(s),
            IdeaMarketStatusType s => GetMarketIdeaStatusInfo(s),
            ProjectStatus s => GetProjectStatusInfo(s),
            HITSTaskStatus s => GetTastStatusInfo(s),
            ProjectMemberRole r => GetProjectMemberRoleInfo(r),
            SprintStatus s => GetSprintStatusInfo(s),
            ProfileModalCategory c => GetProfileCategoryInfo(c),
            ProjectViewCategory c => GetProjectCategoryInfo(c),
            EndedSprintModalStatCategory c => GetSprintModalStatCategoryInfo(c),
            MenuAction a => GetActionMenuInfo(a),
            NotificationType n => GetNotificationTypeInfo(n),
            _ => new EnumUIResult(value?.ToString() ?? string.Empty, string.Empty)
        };
    }
}
