using HITSBlazor.Utils.Attributes;
using System.ComponentModel;

namespace HITSBlazor.Models.Projects.Enums
{
    public enum ProjectStatus
    {
        [Description("Активен")]
        [Style("bg-primary-subtle text-primary")]
        Active,

        [Description("Завершен")]
        [Style("bg-warning-subtle text-warning")]
        Done
    }
}
