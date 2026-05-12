using HITSBlazor.Models.Projects.Enums;

namespace HITSBlazor.Utils.EnumUIConverters
{
    public static partial class EnumUIConverter
    {
        private static EnumUIResult GetProjectStatusInfo(ProjectStatus status) => status switch
        {
            ProjectStatus.Done => new EnumUIResult("Завершен", "bg-warning-subtle text-warning"),
            ProjectStatus.Active => new EnumUIResult("Активен", "bg-primary-subtle text-primary"),
            ProjectStatus.Paused => new EnumUIResult("Остановлен", "bg-primary-subtle text-primary"),
            ProjectStatus.Deleted => new EnumUIResult("Удалён", "bg-danger-subtle text-danger"),
            _ => new EnumUIResult(status.ToString(), string.Empty)
        };
    }
}
