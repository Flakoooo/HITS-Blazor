using HITSBlazor.Models.Quests.Entities;
using HITSBlazor.Utils.Mocks.Teams;

namespace HITSBlazor.Utils.Mocks.Quests
{
    public static class MockQuests
    {
        private static readonly List<Quest> _quests = CreateQuests();

        public static string QuestSpring2024Id { get; } = Guid.NewGuid().ToString();
        public static string QuestAutumn2023Id { get; } = Guid.NewGuid().ToString();
        public static string QuestSpring2023Id { get; } = Guid.NewGuid().ToString();
        public static string QuestAutumn2022Id { get; } = Guid.NewGuid().ToString();

        private static List<Quest> CreateQuests()
        {
            var idTeams = new List<TeamIdWrapper>() { new() { Id = MockTeams.CardId } };

            return
            [
                new Quest
                {
                    IdQuest = QuestSpring2024Id,
                    IdQuestTemplate = MockQuestTemplateShorts.QuestTemplate1Id,
                    IdTeams =
                    [
                        new TeamIdWrapper { Id = MockTeams.CardId },
                        new TeamIdWrapper { Id = MockTeams.CactusId },
                        new TeamIdWrapper { Id = MockTeams.CarpId }
                    ],
                    Name = "Весенний опрос 2024",
                    StartAt = new DateTime(2024, 4, 28).ToString("dd.MM.yyyy"),
                    EndAt = new DateTime(2024, 5, 28).ToString("dd.MM.yyyy"),
                    Available = true,
                    Percent = "22",
                    Passed = false
                },
                new Quest
                {
                    IdQuest = QuestAutumn2023Id,
                    IdQuestTemplate = MockQuestTemplateShorts.QuestTemplate2Id,
                    IdTeams = [.. idTeams],
                    Name = "Осенний опрос 2023",
                    StartAt = new DateTime(2023, 11, 28).ToString("dd.MM.yyyy"),
                    EndAt = new DateTime(2023, 12, 28).ToString("dd.MM.yyyy"),
                    Available = true,
                    Percent = "77",
                    Passed = true
                },
                new Quest
                {
                    IdQuest = QuestSpring2023Id,
                    IdQuestTemplate = MockQuestTemplateShorts.QuestTemplate1Id,
                    IdTeams = [.. idTeams],
                    Name = "Весенний опрос 2023",
                    StartAt = new DateTime(2023, 4, 28).ToString("dd.MM.yyyy"),
                    EndAt = new DateTime(2023, 5, 28).ToString("dd.MM.yyyy"),
                    Available = true,
                    Percent = "65",
                    Passed = true
                },
                new Quest
                {
                    IdQuest = QuestAutumn2022Id,
                    IdQuestTemplate = MockQuestTemplateShorts.QuestTemplate2Id,
                    IdTeams = [.. idTeams],
                    Name = "Осенний опрос 2022",
                    StartAt = new DateTime(2022, 11, 28).ToString("dd.MM.yyyy"),
                    EndAt = new DateTime(2022, 12, 28).ToString("dd.MM.yyyy"),
                    Available = false,
                    Percent = "50",
                    Passed = false
                }
            ];
        }

        public static Quest? GetQuestById(string id) =>
            _quests.FirstOrDefault(q => q.IdQuest == id);
    }
}
