using HITSBlazor.Models.Projects.Enums;

namespace HITSBlazor.Models.Projects.Entities
{
    public class SprintMarks
    {
        public Guid SprintId { get; set; }
        public Guid UserId { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public ProjectMemberRole ProjectRole { get; set; }
        public int? Mark { get; set; }
        public int? CountCompletedTasks { get; set; }
    }
}
