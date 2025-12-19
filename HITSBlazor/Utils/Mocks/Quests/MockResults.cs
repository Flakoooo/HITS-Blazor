using HITSBlazor.Models.Quests.Entities;
using HITSBlazor.Utils.Mocks.Teams;
using HITSBlazor.Utils.Mocks.Users;

namespace HITSBlazor.Utils.Mocks.Quests
{
    public static class MockResults
    {
        private static readonly QuestResultWrapper _results = CreateResults();
        
        private static QuestResultWrapper CreateResults()
        {
            var teamCard = MockTeams.GetTeamById(MockTeams.CardId);
            var kirill = MockUsers.GetUserById(MockUsers.KirillId)!;

            var indicators = MockIndicators.GetAllIndicators();

            var results = new List<QuestResult>
            {
                new() 
                {
                    IdResult = Guid.NewGuid(),
                    IdIndicator = indicators[0].Id,
                    IdQuest = MockQuests.QuestSpring2024Id,
                    IdFromUser = kirill.Id,
                    Value = "4"
                },
                new()
                {
                    IdResult = Guid.NewGuid(),
                    IdIndicator = indicators[1].Id,
                    IdQuest = MockQuests.QuestSpring2024Id,
                    IdFromUser = kirill.Id,
                    IdToUser = Guid.Empty,
                    Value = string.Empty
                }
            };

            if (teamCard?.Members != null)
                foreach (var member in teamCard.Members)
                    results.Add(new QuestResult
                    {
                        IdResult = Guid.NewGuid(),
                        IdIndicator = indicators[2].Id,
                        IdQuest = MockQuests.QuestSpring2024Id,
                        IdFromUser = kirill.Id,
                        IdToUser = member.UserId,
                        Value = string.Empty
                    });

            results.AddRange([
                new QuestResult
                {
                    IdResult = Guid.NewGuid(),
                    IdIndicator = indicators[0].Id,
                    IdQuest = MockQuests.QuestSpring2024Id,
                    IdFromUser = kirill.Id,
                    IdToUser = Guid.Empty,
                    Value = string.Empty
                },
                new QuestResult
                {
                    IdResult = Guid.NewGuid(),
                    IdIndicator = indicators[1].Id,
                    IdQuest = MockQuests.QuestSpring2024Id,
                    IdFromUser = kirill.Id,
                    IdToUser = Guid.Empty,
                    Value = string.Empty
                }
            ]);

            if (teamCard?.Members != null)
                foreach (var member in teamCard.Members)
                    results.Add(new QuestResult
                    {
                        IdResult = Guid.NewGuid(),
                        IdIndicator = indicators[2].Id,
                        IdQuest = MockQuests.QuestSpring2024Id,
                        IdFromUser = kirill.Id,
                        IdToUser = member.UserId,
                        Value = string.Empty
                    });

            return new QuestResultWrapper { Results = results };
        }
    }
}
