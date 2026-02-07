using HITSBlazor.Models.Teams.Enums;

namespace HITSBlazor.Utils.EnumStyles
{
    public static class JoinStatusStyle
    {
        private static readonly Dictionary<JoinStatus, string> Styles = new()
        {
            [JoinStatus.New] = "bg-primary-subtle text-primary",
            [JoinStatus.Annulled] = "bg-primary-subtle text-primary",
            [JoinStatus.Accepted] = "bg-success-subtle text-success",
            [JoinStatus.Canceled] = "bg-danger-subtle text-danger",
            [JoinStatus.Withdrawn] = "bg-warning-subtle text-warning"
        };

        public static string GetStyle(this JoinStatus status)
            => Styles.TryGetValue(status, out var style)
                ? style
                : string.Empty;

        public static string GetStyle(this JoinStatus? status)
            => status is null ? string.Empty : status.Value.GetStyle();
    }
}
