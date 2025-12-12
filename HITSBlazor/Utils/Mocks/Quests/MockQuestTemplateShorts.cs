using HITSBlazor.Models.Quests.Entities;

namespace HITSBlazor.Utils.Mocks.Quests
{
    public static class MockQuestTemplateShorts
    {
        private static readonly List<QuestTemplateShort> _questTemplateShorts = CreateQuestTemplateShorts();

        public static string QuestTemplate1Id { get; } = Guid.NewGuid().ToString();
        public static string QuestTemplate2Id { get; } = Guid.NewGuid().ToString();

        private static List<QuestTemplateShort> CreateQuestTemplateShorts() =>
        [
            new QuestTemplateShort { IdQuestTemplate = QuestTemplate1Id, Available = true, Name = "Опрос компетенций" },
            new QuestTemplateShort { IdQuestTemplate = QuestTemplate2Id, Available = true, Name = "Опрос компетенций" }
        ];

        public static QuestTemplateShort? GetQuestTemplateShortById(string id) =>
            _questTemplateShorts.FirstOrDefault(qts => qts.IdQuestTemplate == id);
    }
}
