using HITSBlazor.Models.Projects.Enums;

namespace HITSBlazor.Utils.EnumUIConverters
{
    public static partial class EnumUIConverter
    {
        private static EnumUIResult GetSprintStatusInfo(SprintStatus status) => status switch
        {
            SprintStatus.Active => new EnumUIResult("Активен", "bg-success-subtle text-success"),
            SprintStatus.Done => new EnumUIResult("Завершен", "bg-warning-subtle text-warning"),
            _ => new EnumUIResult(status.ToString(), string.Empty)
        };
    }
}
