using HITSBlazor.Models.Users.Enums;

namespace HITSBlazor.Utils.EnumTranslators
{
    public static class RoleTypeTranslator
    {
        private static readonly Dictionary<RoleType, string> Translations = new()
        {
            [RoleType.Initiator] = "Инициатор",
            [RoleType.TeamOwner] = "Владелец команды",
            [RoleType.TeamLeader] = "Лидер команды",
            [RoleType.Member] = "Студент",
            [RoleType.ProjectOffice] = "Проектный офис",
            [RoleType.Expert] = "Эксперт",
            [RoleType.Admin] = "Админ",
            [RoleType.Teacher] = "Преподаватель"
        };

        public static string GetTranslation(this RoleType role)
            => Translations.TryGetValue(role, out var translation)
                ? translation
                : role.ToString();

        public static string GetTranslation(this RoleType? role)
            => role is null ? string.Empty : role.Value.GetTranslation();
    }
}
