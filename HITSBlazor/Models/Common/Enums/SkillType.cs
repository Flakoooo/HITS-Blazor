using System.ComponentModel;

namespace HITSBlazor.Models.Common.Enums
{
    public enum SkillType
    {
        [Description("Язык разработки")]
        Language = 0,

        [Description("Фреймворк")]
        Framework = 1,

        [Description("База данных")]
        Database = 2,

        [Description("DevOps технология")]
        Devops = 3
    }
}
