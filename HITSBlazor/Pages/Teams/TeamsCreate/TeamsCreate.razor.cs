using HITSBlazor.Models.Common.Entities;

namespace HITSBlazor.Pages.Teams.TeamsCreate
{
    public partial class TeamsCreate
    {
        private string _value = string.Empty;

        private HashSet<Skill> SelectedLanguageSkills { get; set; } = [];
        private HashSet<Skill> SelectedFrameworkSkills { get; set; } = [];
        private HashSet<Skill> SelectedDatabaseSkills { get; set; } = [];
        private HashSet<Skill> SelectedDevopsSkills { get; set; } = [];
    }
}
