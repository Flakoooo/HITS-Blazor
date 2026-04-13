using HITSBlazor.Utils.Attributes;
using System.ComponentModel;

namespace HITSBlazor.Models.Projects.Enums
{
    public enum TaskStatus
    {
        [Description("В бэклоге")]
        [Style("bg-success-subtle text-success")]
        InBackLog,

        [Description("Новые")]
        [Style("bg-primary-subtle text-primary")]
        NewTask,

        [Description("На выполнении")]
        [Style("bg-warning-subtle text-warning")]
        InProgress,

        [Description("На проверке")]
        [Style("bg-warning-subtle text-warning")]
        OnVerification,

        [Description("На доработке")]
        [Style("bg-warning-subtle text-warning")]
        OnModification,

        [Description("Выполненные")]
        [Style("bg-success-subtle text-success")]
        Done
    }
}
