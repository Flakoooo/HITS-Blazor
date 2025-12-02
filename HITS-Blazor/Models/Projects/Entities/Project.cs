using HITSBlazor.Models.Projects.Enums;

namespace HITSBlazor.Models.Projects.Entities
{
    public class Project
    {
        public string Id { get; set; } = string.Empty;
        public string IdeaId { get; set; } = string.Empty;

        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Customer { get; set; } = string.Empty;
        public User Initiator { get; set; } = new();
        public Team Team { get; set; } = new();
        public List<ProjectMember> Members { get; set; } = new();

        public ReportProject Report { get; set; } = new();
        public string StartDate { get; set; } = string.Empty;
        public string FinishDate { get; set; } = string.Empty;
        public ProjectStatus Status { get; set; }
    }
}
