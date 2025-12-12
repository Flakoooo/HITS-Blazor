using HITSBlazor.Models.Quests.Entities;
using HITSBlazor.Utils.Mocks.Teams;

namespace HITSBlazor.Utils.Mocks.Quests
{
    public static class MockTeamQuestStats
    {
        private static readonly List<TeamQuestStat> teamQuestStats = CreateTeamQuestStats();

        private static List<TeamQuestStat> CreateTeamQuestStats()
        {
            var cardTeam = MockTeams.GetTeamById(MockTeams.CardId)!;
            var cactusTeam = MockTeams.GetTeamById(MockTeams.CactusId)!;
            var carpTeam = MockTeams.GetTeamById(MockTeams.CarpId)!;

            

            return
            [
                new TeamQuestStat
                {
                    Id = cactusTeam.Id,
                    Name = cactusTeam.Name,
                    Progress = $"{2 / 3 * 100}",
                    Users = 
                }
            ];
        }
    }
}
