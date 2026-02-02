using HITSBlazor.Models.Teams.Entities;
using HITSBlazor.Utils.Mocks.Users;

namespace HITSBlazor.Utils.Mocks.Teams
{
    public static class MockTeamExperiences
    {
        private static readonly List<TeamExperience> _teamExperiences = CreateTeamExperiences();

        private static List<TeamExperience> CreateTeamExperiences()
        {
            var cardTeam = MockTeams.GetTeamById(MockTeams.CardId)!;

            var kirill = MockUsers.GetUserById(MockUsers.KirillId)!;

            return
            [
                new TeamExperience
                {
                    TeamId = cardTeam.Id,
                    TeamName = cardTeam.Name,
                    UserId = kirill.Id,
                    FirstName = kirill.FirstName,
                    LastName = kirill.LastName,
                    StartDate = new DateTime(2023, 1, 1, 11, 2, 17, DateTimeKind.Utc),
                    FinishDate = null,
                    HasActiveProject = cardTeam.HasActiveProject
                }
            ];
        }

        public static List<TeamExperience> GetTeamExperiencesByUserId(Guid userId)
            => [.. _teamExperiences.Where(te => te.UserId == userId)];
    }
}
