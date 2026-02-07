using HITSBlazor.Models.Teams.Enums;

namespace HITSBlazor.Utils.EnumTranslators
{
    public static class JoinStatusTranslator
    {
        private static readonly Dictionary<JoinStatus, string> Translations = new()
        {
            [JoinStatus.New] = "Новая",
            [JoinStatus.Annulled] = "Аннулирована",
            [JoinStatus.Accepted] = "Принята",
            [JoinStatus.Canceled] = "Отклонена",
            [JoinStatus.Withdrawn] = "Отозвана"
        };

        public static string GetTranslation(this JoinStatus status)
            => Translations.TryGetValue(status, out var translation)
                ? translation
                : status.ToString();

        public static string GetTranslation(this JoinStatus? status)
            => status is null ? string.Empty : status.Value.GetTranslation();
    }
}
