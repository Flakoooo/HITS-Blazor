using System.ComponentModel;

namespace HITSBlazor.Models.Projects.Enums
{
    public enum ProjectStatus
    {
        [Description("Активен")]
        Active,

        [Description("Завершен")]
        Done
    }
}
