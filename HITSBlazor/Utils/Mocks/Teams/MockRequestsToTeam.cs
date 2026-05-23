using HITSBlazor.Models.Common.Responses;
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
                    Status = TeamRequestStatus.New,
                    Email = maga.Email,
                    FirstName = maga.FirstName,
                    LastName = maga.LastName
                }
            ];
        }

        public static ListDataResponse<RequestToTeam> GetRequestToTeams(
            int page,
            int pageSize = 20,
            Guid? teamId = null,
            string? searchText = null,
            HashSet<TeamRequestStatus>? selectedStatuses = null
        )
        {
            var query = _requestsToTeam.Where(ti => ti.TeamId == teamId).AsQueryable();

            if (selectedStatuses?.Count > 0)
                query = query.Where(rtt => selectedStatuses.Contains(rtt.Status));

            if (!string.IsNullOrWhiteSpace(searchText))
                query = query.Where(rtt => rtt.FullName.Contains(searchText, StringComparison.CurrentCultureIgnoreCase)
                    || rtt.Email.Contains(searchText, StringComparison.CurrentCultureIgnoreCase)
                );

            int count = query.Count();

            query = query.Skip((page - 1) * pageSize).Take(pageSize);

            return new ListDataResponse<RequestToTeam>(count, query.ToList());
        }

        public static bool CreateNewRequest(Guid teamId, Guid userId)
        {
            var currentUser = MockUsers.GetUserById(userId);
            if (currentUser is null) return false;

            var newRequest = new RequestToTeam
            {
                Id = MagaId,
                TeamId = teamId,
                UserId = currentUser.Id,
                Status = TeamRequestStatus.New,
                Email = currentUser.Email,
                FirstName = currentUser.FirstName,
                LastName = currentUser.LastName
            };

            _requestsToTeam.Add(newRequest);

            return true;
        }

        public static bool UpdateRequestStatus(Guid requestId, TeamRequestStatus newStatus)
        {
            var requestForUpdate = _requestsToTeam.FirstOrDefault(rtt => rtt.Id == requestId);
            if (requestForUpdate is null) return false;

            requestForUpdate.Status = newStatus;

            return true;
        }
    }
}
