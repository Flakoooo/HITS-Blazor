using HITSBlazor.Utils.Attributes;
using System.ComponentModel;

namespace HITSBlazor.Models.Projects.Enums
{
    public enum SprintStatus
    {
        [Description("Активен")]
        [Style("bg-success-subtle text-success")]
        Active,

        [Description("Завершен")]
        [Style("bg-warning-subtle text-warning")]
        Done
    }
}
