using HITSBlazor.Utils.Attributes;
using System.ComponentModel;

namespace HITSBlazor.Models.Projects.Enums
{
    public enum TaskStatus
    {
        [Description("")]
        InBackLog,

        [Description("На доработке")]
        [Style("#8a2be2")]
        OnModification,

        [Description("Новые")]
        [Style("#0d6efd")]
        NewTask,

        [Description("На выполнении")]
        [Style("#f5ec0a")]
        InProgress,

        [Description("На проверке")]
        [Style("#ffa800")]
        OnVerification,

        [Description("Выполненные")]
        [Style("#13c63a")]
        Done
    }
}
