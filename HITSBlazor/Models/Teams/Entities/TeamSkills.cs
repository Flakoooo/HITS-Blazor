using HITSBlazor.Models.Common.Entities;

namespace HITSBlazor.Models.Teams.Entities
{
    public class TeamSkills
    {
        public string TeamId { get; set; } = string.Empty;
        public List<Skill> Skills { get; set; } = [];
        public List<Skill> WantedSkills { get; set; } = [];
    }
}
