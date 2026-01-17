using HITSBlazor.Models.Common.Enums;

namespace HITSBlazor.Utils.EnumTranslators
{
    public static class SkillTypeTranslator
    {
        private static readonly Dictionary<SkillType, string> Translations = new()
        {
            [SkillType.Language] = "Языки разработки",
            [SkillType.Framework] = "Фреймворки",
            [SkillType.Database] = "Базы данных",
            [SkillType.Devops] = "DevOps технологии"
        };

        public static string GetTranslation(this SkillType status)
            => Translations.TryGetValue(status, out var translation)
                ? translation
                : "Навыки";

        public static string GetTranslation(this SkillType? status)
            => status is null ? "Навыки" : status.Value.GetTranslation();
    }
}
