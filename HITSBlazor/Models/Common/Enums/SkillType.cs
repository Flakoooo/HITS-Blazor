using System.ComponentModel;

namespace HITSBlazor.Models.Common.Enums
{
    public enum SkillType
    {
        [Description("Язык разработки")]
        Language,

        [Description("Фреймворк")]
        Framework,

        [Description("База данных")]
        Database,

        [Description("DevOps технология")]
        Devops
    }
}
