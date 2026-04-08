using System.ComponentModel;

namespace HITSBlazor.Pages.Projects.ProjectView
{
    public enum ProjectViewCategory
    {
        [Description("О проекте")]
        Info,

        [Description("Бэклог")]
        Backlog,

        [Description("Спринты")]
        Sprints
    }
}
