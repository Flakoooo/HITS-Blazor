namespace HITSBlazor.Models.Quests.Entities
{
    public class QuestTemplateShort
    {
        public Guid IdQuestTemplate { get; set; }
        public bool? Available { get; set; }
        public string Name { get; set; } = string.Empty;
    }
}
