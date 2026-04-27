using HITSBlazor.Pages.Projects.ProjectView;

namespace HITSBlazor.Utils.EnumUIConverters
{
    public static partial class EnumUIConverter
    {
        private static EnumUIResult GetProjectCategoryInfo(ProjectViewCategory status) => status switch
        {
            ProjectViewCategory.Info => new EnumUIResult("О проекте", string.Empty),
            ProjectViewCategory.Backlog => new EnumUIResult("Бэклог", string.Empty),
            ProjectViewCategory.Sprints => new EnumUIResult("Спринты", string.Empty),
            ProjectViewCategory.ActiveSprint => new EnumUIResult("Активный спринт", string.Empty),
            _ => new EnumUIResult(status.ToString(), string.Empty)
        };
    }
}
