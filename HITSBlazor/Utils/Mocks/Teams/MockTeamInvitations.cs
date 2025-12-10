using HITSBlazor.Models.Teams.Entities;
using HITSBlazor.Models.Teams.Enums;
using HITSBlazor.Utils.Mocks.Users;

namespace HITSBlazor.Utils.Mocks.Teams
{
    public static class MockTeamInvitations
    {
        private static readonly List<TeamInvitation> _teamInvitations = CreateTeamInvitations();

        public static string MagaInvitationId { get; } = Guid.NewGuid().ToString();
        public static string KirillInvitationId { get; } = Guid.NewGuid().ToString();
        public static string TimurInvitationId { get; } = Guid.NewGuid().ToString();
        public static string AdminInvitationId { get; } = Guid.NewGuid().ToString();

        private static List<TeamInvitation> CreateTeamInvitations()
        {
            var maga = MockUsers.GetUserById(MockUsers.MagaId)!;

            return [
                new TeamInvitation
                {
                    Id = MagaInvitationId,
                    TeamId = MockTeams.CardId,
                    UserId = maga.Id,
                    Email = maga.Email,
                    Status = JoinStatus.ACCEPTED,
                    FirstName = maga.FirstName,
                    LastName = maga.LastName
                }
            ];
        }
    }
}
