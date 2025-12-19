namespace HITSBlazor.Models.Quests.Entities
{
    public class QuestResult
    {
        public Guid IdResult { get; set; } //nullable
        public Guid IdIndicator { get; set; }
        public Guid IdQuest { get; set; }
        public Guid IdFromUser { get; set; }
        public Guid IdToUser { get; set; } //nullable
        public string Value { get; set; } = string.Empty;
    }
}
