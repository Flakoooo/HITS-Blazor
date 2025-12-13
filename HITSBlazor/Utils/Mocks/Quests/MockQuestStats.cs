using HITSBlazor.Models.Quests.Entities;
using HITSBlazor.Utils.Mocks.Teams;

namespace HITSBlazor.Utils.Mocks.Quests
{
    public static class MockQuestStats
    {
        private static readonly List<QuestStat> _questStats = CreateQuestStats();

        private static string SumProgress(params TeamQuestStat[] stats)
        {
            int sum = 0;
            foreach (var stat in stats)
                if (int.TryParse(stat.Progress, out int progress))
                    sum += progress;

            return (sum / stats.Length).ToString();
        }

        private static List<QuestStat> CreateQuestStats()
        {
            var quest1 = MockQuests.GetQuestById(MockQuests.QuestSpring2024Id)!;
            var quest2 = MockQuests.GetQuestById(MockQuests.QuestAutumn2023Id)!;
            var quest3 = MockQuests.GetQuestById(MockQuests.QuestSpring2023Id)!;
            var quest4 = MockQuests.GetQuestById(MockQuests.QuestAutumn2022Id)!;

            var cardQuestStats = MockTeamQuestStats.GetTeamQuestStatByIt(MockTeams.CardId)!;
            var cactusQuestStats = MockTeamQuestStats.GetTeamQuestStatByIt(MockTeams.CactusId)!;

            return
            [
                new QuestStat
                {
                    IdQuest = quest1.IdQuest,
                    IsPass = true,
                    IdQuestTemplate = quest1.IdQuestTemplate,
                    Name = quest1.Name,
                    Progress = SumProgress(cardQuestStats, cactusQuestStats),
                    Teams = [cardQuestStats, cactusQuestStats]
                },
                new QuestStat
                {
                    IdQuest = quest2.IdQuest,
                    IsPass = true,
                    IdQuestTemplate = quest2.IdQuestTemplate,
                    Name = quest2.Name,
                    Progress = cardQuestStats.Progress,
                    Teams = [cardQuestStats]
                },
                new QuestStat
                {
                    IdQuest = quest3.IdQuest,
                    IsPass = true,
                    IdQuestTemplate = quest3.IdQuestTemplate,
                    Name = quest3.Name,
                    Progress = cardQuestStats.Progress,
                    Teams = [cardQuestStats]
                },
                new QuestStat
                {
                    IdQuest = quest4.IdQuest,
                    IsPass = true,
                    IdQuestTemplate = quest4.IdQuestTemplate,
                    Name = quest4.Name,
                    Progress = cardQuestStats.Progress,
                    Teams = [cardQuestStats]
                }
            ];
        }
    }
}
