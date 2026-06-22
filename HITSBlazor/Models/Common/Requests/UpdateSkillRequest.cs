using HITSBlazor.Models.Common.Enums;

namespace HITSBlazor.Models.Common.Requests
{
    public class UpdateSkillRequest
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public SkillType Type { get; set; }
        public bool Confirmed { get; set; }
    }
}
