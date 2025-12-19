using HITSBlazor.Models.Projects.Entities;

namespace HITSBlazor.Models.Teams.Entities
{
    public class TeamProject
    {
        public Guid TeamId { get; set; }
        public string TeamName { get; set; } = string.Empty;
        public Project Project { get; set; } = new();
    }
}
