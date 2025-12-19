namespace HITSBlazor.Models.Quests.Entities
{
    public class TeamIdWrapper
    {
        public Guid Id { get; set; }
    }

    public class Quest
    {
        public Guid IdQuest { get; set; }
        public Guid IdQuestTemplate { get; set; }
        public List<TeamIdWrapper> IdTeams { get; set; } = [];
        public string Name { get; set; } = string.Empty;
        public string StartAt { get; set; } = string.Empty;
        public string EndAt { get; set; } = string.Empty;
        public bool Available { get; set; }
        public string? Percent { get; set; }
        public bool? Passed { get; set; }
    }
}
