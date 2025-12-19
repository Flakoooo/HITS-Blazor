using HITSBlazor.Models.Common.Entities;

namespace HITSBlazor.Models.Ideas.Entities
{
    public class IdeaSkills
    {
        public Guid IdeaId { get; set; }
        public List<Skill> Skills { get; set; } = [];
    }
}
