using HITSBlazor.Models.Ideas.Enums;

namespace HITSBlazor.Utils.EnumTranslators
{
    public static class IdeaStatusTypeTranslator
    {
        private static readonly Dictionary<IdeaStatusType, string> Translations = new()
        {
            [IdeaStatusType.New] = "Новая",
            [IdeaStatusType.OnEditing] = "На редактировании",
            [IdeaStatusType.OnApproval] = "На согласовании",
            [IdeaStatusType.OnConfirmation] = "На утверждении",
            [IdeaStatusType.Confirmed] = "Утверждена",
            [IdeaStatusType.OnMarket] = "Опубликована"
        };

        public static string GetTranslation(this IdeaStatusType status)
            => Translations.TryGetValue(status, out var translation)
                ? translation
                : status.ToString();

        public static string GetTranslation(this IdeaStatusType? status)
            => status is null ? string.Empty : status.Value.GetTranslation();
    }
}
