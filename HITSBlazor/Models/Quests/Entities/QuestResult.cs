namespace HITSBlazor.Models.Quests.Entities
{
    public class QuestResult
    {
        public string? IdResult { get; set; }
        public string IdIndicator { get; set; } = string.Empty;
        public string IdQuest { get; set; } = string.Empty;
        public string IdFromUser { get; set; } = string.Empty;
        public string? IdToUser { get; set; }
        public string Value { get; set; } = string.Empty;
    }
}
