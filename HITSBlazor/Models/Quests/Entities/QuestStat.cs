namespace HITSBlazor.Models.Quests.Entities
{
    public class QuestStat
    {
        public string IdQuest { get; set; } = string.Empty;
        public bool IsPass { get; set; }
        public string IdQuestTemplate { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Progress { get; set; } = string.Empty;
        public List<TeamQuestStat> Teams { get; set; } = [];
    }
}
