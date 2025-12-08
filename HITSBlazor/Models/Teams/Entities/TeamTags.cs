using HITSBlazor.Models.Teams.Enums;

namespace HITSBlazor.Models.Teams.Entities
{
    public class TeamTags
    {
        public List<string> StudyGroups { get; set; } = [];
        public List<Course> StudyCourses { get; set; } = [];
    }
}
