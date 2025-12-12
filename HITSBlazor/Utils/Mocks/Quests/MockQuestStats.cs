using HITSBlazor.Models.Quests.Entities;
using HITSBlazor.Utils.Mocks.Teams;

namespace HITSBlazor.Utils.Mocks.Quests
{
    public static class MockQuestStats
    {
        private static readonly List<QuestStat> _questStats = CreateQuestStats();

        private static List<QuestStat> CreateQuestStats()
        {
            var usersStats = CreateUsersQuestStats();
            var teamsStats = CreateTeamsQuestStats();

            var teams = MockTeams.GetAllTeams();

            var quest1 = MockQuests.GetQuestById(MockQuests.QuestSpring2024Id)!;

            return
            [
                new QuestStat
                {
                    IdQuest = quest1.IdQuest,
                    IsPass = true,
                    IdQuestTemplate = quest1.IdQuestTemplate,
                    Name = quest1.Name,
                    Progress = CalculateAverageProgress(teamsStats.Take(3).ToList()),
                    Teams = [.. teams.Take(3)]
                },
                new QuestStat
                {
                    IdQuest = MockQuests.QuestSpring2023Id,
                    IsPass = true,
                    IdQuestTemplate = Template2Id,
                    Name = "Осенний опрос 2023",
                    Progress = teamsStats[0].Progress,
                    Teams = new List<TeamQuestStat> { teamsStats[0] }
                }
                // Добавьте остальные статистики по аналогии
            ];
        }
    }
}
