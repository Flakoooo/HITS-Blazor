using HITSBlazor.Models.Teams.Enums;

namespace HITSBlazor.Utils.EnumStyles
{
    public static class TeamRequestStatusStyle
    {
        private static readonly Dictionary<TeamRequestStatus, string> Styles = new()
        {
            [TeamRequestStatus.New] = "bg-primary-subtle text-primary",
            [TeamRequestStatus.Annulled] = "bg-primary-subtle text-primary",
            [TeamRequestStatus.Accepted] = "bg-success-subtle text-success",
            [TeamRequestStatus.Canceled] = "bg-danger-subtle text-danger",
            [TeamRequestStatus.Withdrawn] = "bg-warning-subtle text-warning"
        };

        public static string GetStyle(this TeamRequestStatus status)
            => Styles.TryGetValue(status, out var style)
                ? style
                : string.Empty;

        public static string GetStyle(this TeamRequestStatus? status)
            => status is null ? string.Empty : status.Value.GetStyle();
    }
}
