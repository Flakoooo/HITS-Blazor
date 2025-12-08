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

        public static string MagaUserId { get; } = Guid.NewGuid().ToString();

        public static List<TeamInvitation> GetMockTeamInvitations() => [
            //TODO: Добавить магу в базу данных моков
            new TeamInvitation
            {
                Id = MagaInvitationId,
                TeamId = MockTeams.CardId,
                UserId = MagaUserId,
                Email = "maga@mail.com",
                Status = JoinStatus.ACCEPTED,
                FirstName = "Мамедага",
                LastName = "Байрамов"
            },
            //TODO: не соотвествуют правилу 1 пользователь = 1 команда
            new TeamInvitation
            {
                Id = KirillInvitationId,
                TeamId = "4",
                UserId = MockUsers.KirillId,
                Email = "kirill.vlasov.05@inbox.ru",
                Status = JoinStatus.NEW,
                FirstName = "Кирилл",
                LastName = "Власов"
            },
            new TeamInvitation
            {
                Id = TimurInvitationId,
                TeamId = "0",
                UserId = MockUsers.TimurId,
                Status = JoinStatus.ACCEPTED,
                Email = "timyr@mail.com",
                FirstName = "Тимур",
                LastName = "Минязев"
            },
            new TeamInvitation
            {
                Id = AdminInvitationId,
                TeamId = MockTeams.CardId,
                UserId = MockUsers.AdminId,
                Status = JoinStatus.ACCEPTED,
                Email = "admin@mail.com",
                FirstName = "Админ",
                LastName = "Иванов"
            }
        ];

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
