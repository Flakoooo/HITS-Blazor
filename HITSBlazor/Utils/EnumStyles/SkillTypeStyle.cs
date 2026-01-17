using HITSBlazor.Models.Common.Enums;

namespace HITSBlazor.Utils.EnumStyles
{
    public static class SkillTypeStyle
    {
        private static readonly Dictionary<SkillType, string> Styles = new()
        {
            [SkillType.Language] = "bg-success-subtle text-success",
            [SkillType.Framework] = "bg-info-subtle text-info",
            [SkillType.Database] = "bg-warning-subtle text-warning",
            [SkillType.Devops] = "bg-danger-subtle text-danger"
        };

        public static string GetStyle(this SkillType status)
            => Styles.TryGetValue(status, out var style)
                ? style
                : string.Empty;

        public static string GetStyle(this SkillType? status)
            => status is null ? string.Empty : status.Value.GetStyle();
    }
}
