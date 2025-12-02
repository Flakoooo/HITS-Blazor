using HITSBlazor.Models.Teams.Enums;

namespace HITSBlazor.Models.Teams.Entities
{
    public class TeamTags
    {
        public List<string> StudyGroups { get; set; } = new();
        public List<Course> StudyCourses { get; set; } = new();
    }
}
