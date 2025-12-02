namespace HITSBlazor.Models.Quests.Entities
{
    public class TeamIdWrapper
    {
        public string Id { get; set; } = string.Empty;
    }

    public class Quest
    {
        public string IdQuest { get; set; } = string.Empty;
        public string IdQuestTemplate { get; set; } = string.Empty;
        public List<TeamIdWrapper> IdTeams { get; set; } = new();
        public string Name { get; set; } = string.Empty;
        public string StartAt { get; set; } = string.Empty;
        public string EndAt { get; set; } = string.Empty;
        public bool Available { get; set; }
        public string? Percent { get; set; }
        public bool? Passed { get; set; }
    }

}
