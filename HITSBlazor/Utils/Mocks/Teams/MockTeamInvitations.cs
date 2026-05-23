using HITSBlazor.Models.Common.Responses;
using HITSBlazor.Models.Teams.Entities;
using HITSBlazor.Models.Teams.Enums;
using HITSBlazor.Models.Users.Entities;
using HITSBlazor.Utils.Mocks.Users;

namespace HITSBlazor.Utils.Mocks.Teams
{
    public static class MockTeamInvitations
    {
        private static readonly List<TeamInvitation> _teamInvitations = CreateTeamInvitations();

        public static Guid MagaInvitationId { get; } = Guid.NewGuid();
        public static Guid KirillInvitationId { get; } = Guid.NewGuid();
        public static Guid TimurInvitationId { get; } = Guid.NewGuid();
        public static Guid AdminInvitationId { get; } = Guid.NewGuid();

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
                    Status = TeamRequestStatus.Accepted,
                    FirstName = maga.FirstName,
                    LastName = maga.LastName
                }
            ];
        }

        public static ListDataResponse<TeamInvitation> GetTeamInvitations(
            int page, 
            int pageSize = 20, 
            Guid? teamId = null,
            string? searchText = null,
            HashSet<TeamRequestStatus>? selectedStatuses = null
        )
        {
            var query = _teamInvitations.Where(ti => ti.TeamId == teamId).AsQueryable();

            if (selectedStatuses?.Count > 0)
                query = query.Where(ti => selectedStatuses.Contains(ti.Status));

            if (!string.IsNullOrWhiteSpace(searchText))
                query = query.Where(ti => ti.FullName.Contains(searchText, StringComparison.CurrentCultureIgnoreCase)
                    || ti.Email.Contains(searchText, StringComparison.CurrentCultureIgnoreCase)
                );

            int count = query.Count();

            query = query.Skip((page - 1) * pageSize).Take(pageSize);

            return new ListDataResponse<TeamInvitation>(count, query.ToList());
        }

        public static bool CreateNewInvitations(Guid teamId, IEnumerable<Guid> invitationMemberIds)
        {
            var newInvitations = new List<TeamInvitation>();

            foreach (var memberId in invitationMemberIds)
            {
                var user = MockUsers.GetUserById(memberId);
                if (user is null) continue;

                var newInvitation = new TeamInvitation
                {
                    Id = Guid.NewGuid(),
                    TeamId = teamId,
                    UserId = user.Id,
                    Email = user.Email,
                    Status = TeamRequestStatus.New,
                    FirstName = user.FirstName,
                    LastName = user.LastName
                };

                newInvitations.Add(newInvitation);
            }

            _teamInvitations.AddRange(newInvitations);

            return true;
        }

        public static bool UpdateTeamInvitationStatus(Guid teamInvitationId, TeamRequestStatus newStatus)
        {
            var invitationForUpdate = _teamInvitations.FirstOrDefault(ti => ti.Id == teamInvitationId);
            if (invitationForUpdate is null) return false;

            invitationForUpdate.Status = newStatus;
            return true;
        }
    }
}
