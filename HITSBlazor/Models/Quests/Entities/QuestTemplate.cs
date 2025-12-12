namespace HITSBlazor.Models.Quests.Entities
{
    public class QuestTemplate : QuestTemplateShort
    {
        public string Description { get; set; } = string.Empty;
        public List<Indicator> Indicators { get; set; } = [];
    }
}
