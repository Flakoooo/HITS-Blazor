using HITSBlazor.Models.Teams.Entities;
using HITSBlazor.Models.Teams.Enums;
using HITSBlazor.Utils.Mocks.Users;

namespace HITSBlazor.Utils.Mocks.Teams
{
    public static class MockRequestsToTeam
    {
        public static string MagaId { get; } = Guid.NewGuid().ToString();

        public static List<RequestToTeam> GetMockRequestsToTeam()
        {
            var maga = MockUsers.GetUserById(MockUsers.MagaId);

            return [
                new RequestToTeam
                {
                    Id = MagaId,
                    TeamId = MockTeams.CactusId,
                    UserId = maga.Id,
                    Status = JoinStatus.NEW,
                    Email = maga.Email,
                    FirstName = maga.FirstName,
                    LastName = maga.LastName
                }
            ];
        }

        public static RequestToTeam? GetRequestById(string id)
            => GetMockRequestsToTeam().FirstOrDefault(r => r.Id == id);

        public static List<RequestToTeam> GetRequestsByTeamId(string teamId)
            => [.. GetMockRequestsToTeam().Where(r => r.TeamId == teamId)];

        public static List<RequestToTeam> GetRequestsByUserId(string userId)
            => [.. GetMockRequestsToTeam().Where(r => r.UserId == userId)];

        public static List<RequestToTeam> GetRequestsByEmail(string email)
            => [.. GetMockRequestsToTeam().Where(r => r.Email == email)];

        public static List<RequestToTeam> GetRequestsByStatus(JoinStatus status)
            => [.. GetMockRequestsToTeam().Where(r => r.Status == status)];

        public static List<RequestToTeam> GetPendingRequests()
            => GetRequestsByStatus(JoinStatus.NEW);

        public static List<RequestToTeam> GetAcceptedRequests()
            => GetRequestsByStatus(JoinStatus.ACCEPTED);

        public static List<RequestToTeam> GetCanceledRequests()
            => GetRequestsByStatus(JoinStatus.CANCELED);

        public static List<RequestToTeam> GetAnnulledRequests()
            => GetRequestsByStatus(JoinStatus.ANNULLED);

        public static List<RequestToTeam> GetWithdrawnRequests()
            => GetRequestsByStatus(JoinStatus.WITHDRAWN);

        public static bool HasPendingRequest(string userId, string teamId)
            => GetMockRequestsToTeam().Any(r =>
                r.UserId == userId &&
                r.TeamId == teamId &&
                r.Status == JoinStatus.NEW);

        public static bool HasAcceptedRequest(string userId, string teamId)
            => GetMockRequestsToTeam().Any(r =>
                r.UserId == userId &&
                r.TeamId == teamId &&
                r.Status == JoinStatus.ACCEPTED);

        public static string GetFullName(this RequestToTeam request)
            => $"{request.FirstName} {request.LastName}";
    }
}
