using HITSBlazor.Models.Ideas.Enums;

namespace HITSBlazor.Utils.EnumStyles
{
    public static class IdeaStatusTypeStyle
    {
        private static readonly Dictionary<IdeaStatusType, string> Styles = new()
        {
            [IdeaStatusType.New] = "bg-primary-subtle text-primary",
            [IdeaStatusType.OnEditing] = "bg-warning-subtle text-warning",
            [IdeaStatusType.OnApproval] = "bg-warning-subtle text-warning",
            [IdeaStatusType.OnConfirmation] = "bg-danger-subtle text-danger",
            [IdeaStatusType.Confirmed] = "bg-success-subtle text-success",
            [IdeaStatusType.OnMarket] = "bg-success-subtle text-success"
        };

        public static string GetStyle(this IdeaStatusType status)
            => Styles.TryGetValue(status, out var style)
                ? style
                : string.Empty;
    }
}
