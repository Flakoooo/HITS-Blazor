using HITSBlazor.Models.Teams.Entities;
using HITSBlazor.Models.Teams.Enums;
using HITSBlazor.Utils.Mocks.Users;

namespace HITSBlazor.Utils.Mocks.Teams
{
    public static class MockRequestsToTeam
    {
        private static readonly List<RequestToTeam> _requestsToTeam = CreateRequestsToTeam();

        public static Guid MagaId { get; } = Guid.NewGuid();

        private static List<RequestToTeam> CreateRequestsToTeam()
        {
            var maga = MockUsers.GetUserById(MockUsers.MagaId)!;

            return [
                new RequestToTeam
                {
                    Id = MagaId,
                    TeamId = MockTeams.CactusId,
                    UserId = maga.Id,
                    Status = JoinStatus.New,
                    Email = maga.Email,
                    FirstName = maga.FirstName,
                    LastName = maga.LastName
                }
            ];
        }
    }
}
