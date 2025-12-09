using HITSBlazor.Models.Teams.Entities;
using HITSBlazor.Models.Teams.Enums;
using HITSBlazor.Utils.Mocks.Users;

namespace HITSBlazor.Utils.Mocks.Teams
{
    public static class MockTeamInvitations
    {
        public static string MagaInvitationId { get; } = Guid.NewGuid().ToString();
        public static string KirillInvitationId { get; } = Guid.NewGuid().ToString();
        public static string TimurInvitationId { get; } = Guid.NewGuid().ToString();
        public static string AdminInvitationId { get; } = Guid.NewGuid().ToString();

        public static List<TeamInvitation> GetMockTeamInvitations()
        {
            var maga = MockUsers.GetUserById(MockUsers.MagaId);

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

        public static TeamInvitation? GetInvitationById(string id)
            => GetMockTeamInvitations().FirstOrDefault(i => i.Id == id);

        public static List<TeamInvitation> GetInvitationsByTeamId(string teamId)
            => [.. GetMockTeamInvitations().Where(i => i.TeamId == teamId)];

        public static List<TeamInvitation> GetInvitationsByUserId(string userId)
            => [.. GetMockTeamInvitations().Where(i => i.UserId == userId)];

        public static List<TeamInvitation> GetInvitationsByEmail(string email)
            => [.. GetMockTeamInvitations().Where(i => i.Email == email)];

        public static List<TeamInvitation> GetInvitationsByStatus(JoinStatus status)
            => [.. GetMockTeamInvitations().Where(i => i.Status == status)];

        public static List<TeamInvitation> GetPendingInvitations()
            => GetInvitationsByStatus(JoinStatus.NEW);

        public static List<TeamInvitation> GetAcceptedInvitations()
            => GetInvitationsByStatus(JoinStatus.ACCEPTED);

        public static List<TeamInvitation> GetCanceledInvitations()
            => GetInvitationsByStatus(JoinStatus.CANCELED);

        public static List<TeamInvitation> GetAnnulledInvitations()
            => GetInvitationsByStatus(JoinStatus.ANNULLED);

        public static List<TeamInvitation> GetWithdrawnInvitations()
            => GetInvitationsByStatus(JoinStatus.WITHDRAWN);

        public static bool HasPendingInvitation(string userId, string teamId)
            => GetMockTeamInvitations().Any(i =>
                i.UserId == userId &&
                i.TeamId == teamId &&
                i.Status == JoinStatus.NEW);

        public static bool HasAcceptedInvitation(string userId, string teamId)
            => GetMockTeamInvitations().Any(i =>
                i.UserId == userId &&
                i.TeamId == teamId &&
                i.Status == JoinStatus.ACCEPTED);

        public static string GetFullName(this TeamInvitation invitation)
            => $"{invitation.FirstName} {invitation.LastName}";
    }
}
