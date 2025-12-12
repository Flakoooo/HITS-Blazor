using HITSBlazor.Models.Quests.Entities;

namespace HITSBlazor.Utils.Mocks.Quests
{
    public static class MockQuestTemplates
    {
        private static readonly List<QuestTemplate> _questTemplates = CreateQuestTemplates();

        private static List<QuestTemplate> CreateQuestTemplates()
        {
            var indicatorsMocks = MockIndicators.GetAllIndicators().Take(7);

            var quest1 = MockQuestTemplateShorts.GetQuestTemplateShortById(MockQuestTemplateShorts.QuestTemplate1Id)!;
            var quest2 = MockQuestTemplateShorts.GetQuestTemplateShortById(MockQuestTemplateShorts.QuestTemplate2Id)!;

            return
            [
                new QuestTemplate
                {
                    IdQuestTemplate = quest1.IdQuestTemplate,
                    Available = quest1.Available,
                    Name = quest1.Name,
                    Description = "Весенний опрос 2024 посвящен весне и птичкам",
                    Indicators = [.. indicatorsMocks]
                },
                new QuestTemplate
                {
                    IdQuestTemplate = quest2.IdQuestTemplate,
                    Available = quest2.Available,
                    Name = quest2.Name,
                    Description = "Осенний опрос 2024 посвящен весне и птичкам",
                    Indicators = [.. indicatorsMocks]
                }
            ];
        }
    }
}
