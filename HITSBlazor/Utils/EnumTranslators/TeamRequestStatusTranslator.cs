using HITSBlazor.Models.Teams.Enums;

namespace HITSBlazor.Utils.EnumTranslators
{
    public static class TeamRequestStatusTranslator
    {
        private static readonly Dictionary<TeamRequestStatus, string> Translations = new()
        {
            [TeamRequestStatus.New] = "Новая",
            [TeamRequestStatus.Annulled] = "Аннулирована",
            [TeamRequestStatus.Accepted] = "Принята",
            [TeamRequestStatus.Canceled] = "Отклонена",
            [TeamRequestStatus.Withdrawn] = "Отозвана"
        };

        public static string GetTranslation(this TeamRequestStatus status)
            => Translations.TryGetValue(status, out var translation)
                ? translation
                : status.ToString();

        public static string GetTranslation(this TeamRequestStatus? status)
            => status is null ? string.Empty : status.Value.GetTranslation();
    }
}
