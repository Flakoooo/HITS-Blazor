using HITSBlazor.Models.Projects.Enums;
using HITSBlazor.Models.Teams.Entities;
using HITSBlazor.Models.Users.Entities;

namespace HITSBlazor.Models.Projects.Entities
{
    public class Project
    {
        public Guid Id { get; set; }
        public Guid IdeaId { get; set; }

        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Customer { get; set; } = string.Empty;
        public User Initiator { get; set; } = new();
        public Team Team { get; set; } = new();
        public List<ProjectMember> Members { get; set; } = [];

        public string? Report { get; set; }
        public DateOnly StartDate { get; set; }
        public DateOnly FinishDate { get; set; }
        public ProjectStatus Status { get; set; }
    }
}
