using HITSBlazor.Models.Common.Entities;

namespace HITSBlazor.Models.Ideas.Entities
{
    public class IdeaSkills
    {
        public string IdeaId { get; set; } = string.Empty;
        public List<Skill> Skills { get; set; } = [];
    }
}
