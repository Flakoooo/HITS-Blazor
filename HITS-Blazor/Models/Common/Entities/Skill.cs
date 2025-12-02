using HITSBlazor.Models.Common.Enums;

namespace HITSBlazor.Models.Common.Entities
{
    public class Skill
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public SkillType Type { get; set; }
        public bool Confirmed { get; set; }
        public string? CreatorId { get; set; }
        public string? UpdaterId { get; set; }
        public string? DeleterId { get; set; }
    }
}
