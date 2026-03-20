using HITSBlazor.Utils.Attributes;
using System.ComponentModel;

namespace HITSBlazor.Models.Common.Enums
{
    public enum SkillType
    {
        [Description("Язык разработки")]
        [Style("bg-success-subtle text-success")]
        Language = 0,

        [Description("Фреймворк")]
        [Style("bg-info-subtle text-info")]
        Framework = 1,

        [Description("База данных")]
        [Style("bg-warning-subtle text-warning")]
        Database = 2,

        [Description("DevOps технология")]
        [Style("bg-danger-subtle text-danger")]
        Devops = 3
    }
}
