namespace HITSBlazor.Models.Quests.Entities
{
    public class TeamQuestStat
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Progress { get; set; } = string.Empty;
        public List<UsersQuestStat> Users { get; set; } = [];
    }
}
