using HITSBlazor.Models.Quests.Entities;
using HITSBlazor.Utils.Mocks.Teams;
using HITSBlazor.Utils.Mocks.Users;

namespace HITSBlazor.Utils.Mocks.Quests
{
    public static class MockTeamQuestStats
    {
        private static readonly List<TeamQuestStat> _teamQuestStats = CreateTeamQuestStats();

        private static List<TeamQuestStat> CreateTeamQuestStats()
        {
            var cardTeam = MockTeams.GetTeamById(MockTeams.CardId)!;
            var cactusTeam = MockTeams.GetTeamById(MockTeams.CactusId)!;            

            return
            [
                new TeamQuestStat
                {
                    Id = cardTeam.Id,
                    Name = cardTeam.Name,
                    Progress = $"{2 / 3 * 100}",
                    Users = 
                    [
                        MockUsersQuestStats.GetUsersQuestStatById(MockUsers.KirillId)!,
                        MockUsersQuestStats.GetUsersQuestStatById(MockUsers.DenisId)!
                    ]
                },
                new TeamQuestStat
                {
                    Id = cactusTeam.Id,
                    Name = cactusTeam.Name,
                    Progress = $"{2 / 3 * 100}",
                    Users =
                    [
                        MockUsersQuestStats.GetUsersQuestStatById(MockUsers.TimurId)!,
                        MockUsersQuestStats.GetUsersQuestStatById(MockUsers.AdminId)!
                    ]
                }
            ];
        }

        public static TeamQuestStat? GetTeamQuestStatByIt(Guid teamId) =>
            _teamQuestStats.FirstOrDefault(tqs => tqs.Id == teamId);
    }
}
