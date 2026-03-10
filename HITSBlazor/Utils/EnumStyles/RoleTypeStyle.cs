using HITSBlazor.Models.Users.Enums;

namespace HITSBlazor.Utils.EnumStyles
{
    public static class RoleTypeStyle
    {
        private static readonly Dictionary<RoleType, string> Styles = new()
        {
            [RoleType.Initiator] = "bg-primary-subtle text-primary",
            [RoleType.TeamOwner] = "bg-primary-subtle text-primary",
            [RoleType.TeamLeader] = "bg-primary-subtle text-primary",
            [RoleType.Member] = "bg-primary-subtle text-primary",
            [RoleType.ProjectOffice] = "bg-success-subtle text-success",
            [RoleType.Expert] = "bg-success-subtle text-success",
            [RoleType.Teacher] = "bg-success-subtle text-success",
            [RoleType.Admin] = "bg-danger-subtle text-danger"
        };

        public static string GetStyle(this RoleType status)
            => Styles.TryGetValue(status, out var style)
                ? style
                : string.Empty;

        public static string GetStyle(this RoleType? status)
            => status is null ? string.Empty : status.Value.GetStyle();
    }
}
