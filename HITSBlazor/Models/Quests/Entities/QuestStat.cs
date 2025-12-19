namespace HITSBlazor.Models.Quests.Entities
{
    public class QuestStat
    {
        public Guid IdQuest { get; set; }
        public bool IsPass { get; set; }
        public Guid IdQuestTemplate { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Progress { get; set; } = string.Empty;
        public List<TeamQuestStat> Teams { get; set; } = [];
    }
}
