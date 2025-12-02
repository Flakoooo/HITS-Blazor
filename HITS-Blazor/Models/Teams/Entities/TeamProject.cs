using HITSBlazor.Models.Projects.Entities;

namespace HITSBlazor.Models.Teams.Entities
{
    public class TeamProject
    {
        public string TeamId { get; set; } = string.Empty;
        public string TeamName { get; set; } = string.Empty;
        public Project Project { get; set; } = new();
    }
}
