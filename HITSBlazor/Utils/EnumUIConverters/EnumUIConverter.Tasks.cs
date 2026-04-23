using HITSTaskStatus = HITSBlazor.Models.Projects.Enums.TaskStatus;

namespace HITSBlazor.Utils.EnumUIConverters
{
    public static partial class EnumUIConverter
    {
        private static EnumUIResult GetTastStatusInfo(HITSTaskStatus status) => status switch
        {
            HITSTaskStatus.NewTask => new EnumUIResult("Новые", "bg-primary-subtle text-primary"),
            HITSTaskStatus.InBackLog => new EnumUIResult("В бэклоге", "bg-success-subtle text-success"),
            HITSTaskStatus.Done => new EnumUIResult("Выполненные", "bg-success-subtle text-success"),
            HITSTaskStatus.InProgress => new EnumUIResult("На выполнении", "bg-warning-subtle text-warning"),
            HITSTaskStatus.OnVerification => new EnumUIResult("На проверке", "bg-warning-subtle text-warning"),
            HITSTaskStatus.OnModification => new EnumUIResult("На доработке", "bg-warning-subtle text-warning"),
            _ => new EnumUIResult(status.ToString(), string.Empty)
        };
    }
}
