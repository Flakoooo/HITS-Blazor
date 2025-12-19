using HITSBlazor.Models.Common.Entities;

namespace HITSBlazor.Models.Teams.Entities
{
    public class TeamSkills
    {
        public Guid TeamId { get; set; }
        public List<Skill> Skills { get; set; } = [];
        public List<Skill> WantedSkills { get; set; } = [];
    }
}
