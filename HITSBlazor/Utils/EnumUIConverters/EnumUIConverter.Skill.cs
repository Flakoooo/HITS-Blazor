using HITSBlazor.Models.Common.Enums;

namespace HITSBlazor.Utils.EnumUIConverters
{
    public static partial class EnumUIConverter
    {
        private static EnumUIResult GetSkillTypeInfo(SkillType type) => type switch
        {
            SkillType.Language => new EnumUIResult("Язык разработки", "bg-success-subtle text-success"),
            SkillType.Framework => new EnumUIResult("Фреймворк", "bg-info-subtle text-info"),
            SkillType.Database => new EnumUIResult("База данных", "bg-warning-subtle text-warning"),
            SkillType.Devops => new EnumUIResult("DevOps технология", "bg-danger-subtle text-danger"),
            _ => new EnumUIResult(type.ToString(), string.Empty)
        };
    }
}
