using HITSBlazor.Models.Quests.Enums;

namespace HITSBlazor.Models.Quests.Entities
{
    public class Indicator
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public List<string> Answers { get; set; } = new();
        public IndicatorType Type { get; set; }
        public IndicatorRoleType Role { get; set; }
        public bool Visible { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        public string IdCategory { get; set; } = string.Empty;
        public string? IdToUser { get; set; }
    }

}
