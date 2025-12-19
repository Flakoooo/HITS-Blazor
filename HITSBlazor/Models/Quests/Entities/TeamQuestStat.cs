namespace HITSBlazor.Models.Quests.Entities
{
    public class TeamQuestStat
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Progress { get; set; } = string.Empty;
        public List<UsersQuestStat> Users { get; set; } = [];
    }
}
