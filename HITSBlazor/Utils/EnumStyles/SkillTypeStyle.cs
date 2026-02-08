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

        private static readonly Dictionary<SkillType, string> Colors = new()
        {
            [SkillType.Language] = "#d2e7dd",
            [SkillType.Framework] = "#d0f5fc",
            [SkillType.Database] = "#fff3ce",
            [SkillType.Devops] = "#f8d7db"
        };

        private static readonly Dictionary<SkillType, string> WantedColors = new()
        {
            [SkillType.Language] = "#8ac1a7",
            [SkillType.Framework] = "#84e4f7",
            [SkillType.Database] = "#ffdf81",
            [SkillType.Devops] = "#ed98a0"
        };

        public static string GetStyle(this SkillType status)
            => Styles.TryGetValue(status, out var style)
                ? style
                : string.Empty;

        public static string GetStyle(this SkillType? status)
            => status is null ? string.Empty : status.Value.GetStyle();

        public static string GetColor(this SkillType status)
            => Colors.TryGetValue(status, out var color)
                ? color
                : string.Empty;

        public static string GetColor(this SkillType? status)
            => status is null ? string.Empty : status.Value.GetColor();

        public static string GetWantedColor(this SkillType status)
            => WantedColors.TryGetValue(status, out var color)
                ? color
                : string.Empty;

        public static string GetWantedColor(this SkillType? status)
            => status is null ? string.Empty : status.Value.GetWantedColor();
    }
}
