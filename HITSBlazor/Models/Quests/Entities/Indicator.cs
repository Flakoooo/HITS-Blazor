using HITSBlazor.Models.Quests.Enums;

namespace HITSBlazor.Models.Quests.Entities
{
    public class Indicator
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public List<string> Answers { get; set; } = [];
        public IndicatorType Type { get; set; }
        public IndicatorRoleType Role { get; set; }
        public bool Visible { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        public Guid IdCategory { get; set; }
        public Guid? IdToUser { get; set; }
    }
}
