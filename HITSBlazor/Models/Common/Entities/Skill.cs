using HITSBlazor.Models.Common.Enums;

namespace HITSBlazor.Models.Common.Entities
{
    public class Skill
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public SkillType Type { get; set; }
        public bool Confirmed { get; set; }
        public Guid? CreatorId { get; set; }
        public Guid? UpdaterId { get; set; }
        public Guid? DeleterId { get; set; }
    }
}
