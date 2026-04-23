using HITSBlazor.Components.Modals.CenterModals.EndedSprintModal;

namespace HITSBlazor.Utils.EnumUIConverters
{
    public static partial class EnumUIConverter
    {
        private static EnumUIResult GetSprintModalStatCategoryInfo(EndedSprintModalStatCategory category) => category switch
        {
            EndedSprintModalStatCategory.GeneralStat => new EnumUIResult("Общая статистика", string.Empty),
            EndedSprintModalStatCategory.TaskStat => new EnumUIResult("Статистика задачи", string.Empty),
            _ => new EnumUIResult(category.ToString(), string.Empty)
        };
    }
}
