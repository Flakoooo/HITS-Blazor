using HITSBlazor.Models.Projects.Enums;

namespace HITSBlazor.Models.Projects.Entities
{
    public class ProjectMember
    {
        public Guid TeamId { get; set; }
        public string TeamName { get; set; } = string.Empty;
        public string ProjectName { get; set; } = string.Empty;
        public Guid UserId { get; set; }
        public string Email { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public ProjectMemberRole ProjectRole { get; set; }
        public string StartDate { get; set; } = string.Empty;
        public string FinishDate { get; set; } = string.Empty;
    }
}
