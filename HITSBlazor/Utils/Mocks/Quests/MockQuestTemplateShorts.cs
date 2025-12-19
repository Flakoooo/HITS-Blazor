using HITSBlazor.Models.Quests.Entities;

namespace HITSBlazor.Utils.Mocks.Quests
{
    public static class MockQuestTemplateShorts
    {
        private static readonly List<QuestTemplateShort> _questTemplateShorts = CreateQuestTemplateShorts();

        public static Guid QuestTemplate1Id { get; } = Guid.NewGuid();
        public static Guid QuestTemplate2Id { get; } = Guid.NewGuid();

        private static List<QuestTemplateShort> CreateQuestTemplateShorts() =>
        [
            new QuestTemplateShort { IdQuestTemplate = QuestTemplate1Id, Available = true, Name = "Опрос компетенций" },
            new QuestTemplateShort { IdQuestTemplate = QuestTemplate2Id, Available = true, Name = "Опрос компетенций" }
        ];

        public static QuestTemplateShort? GetQuestTemplateShortById(Guid id) =>
            _questTemplateShorts.FirstOrDefault(qts => qts.IdQuestTemplate == id);
    }
}
