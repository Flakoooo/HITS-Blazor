using HITSBlazor.Models.Ideas.Entities;
using HITSBlazor.Models.Teams.Entities;
using HITSBlazor.Utils.Mocks.Ideas;
using HITSBlazor.Utils.Mocks.Teams;

namespace HITSBlazor.Services.Teams
{
    public class MockTeamService(GlobalNotificationService globalNotificationService) : ITeamService
    {
        private readonly GlobalNotificationService _globalNotificationService = globalNotificationService;

        private List<Team> _cachedTeams = [];
        private DateTime _lastRefreshTime;
        private readonly TimeSpan _cacheLifetime = TimeSpan.FromMinutes(5);

        private async Task RefreshCacheAsync()
        {
            _cachedTeams = MockTeams.GetAllTeams();
            _lastRefreshTime = DateTime.UtcNow;
        }

        public async Task<List<Team>> GetTeamsAsync(
            string? searchText = null
        )
        {
            if (_cachedTeams.Count == 0 || DateTime.UtcNow - _lastRefreshTime > _cacheLifetime)
                await RefreshCacheAsync();

            var query = _cachedTeams.AsEnumerable();

            if (!string.IsNullOrWhiteSpace(searchText))
                query = query.Where(i => i.Name.Contains(searchText, StringComparison.CurrentCultureIgnoreCase));

            return [.. query];
        }

        public async Task<Team?> GetTeamByIdAsync(Guid teamId) 
            => MockTeams.GetTeamById(teamId);

        public async Task<bool> DeleteTeamAsync(Team team)
        {
            if (!MockTeams.DeleteTeam(team))
            {
                _globalNotificationService.ShowError("Не удалось удалить идею");
                return false;
            }

            _cachedTeams.Remove(team);
            return true;
        }

        public async Task<List<TeamInvitation>> GetTeamInvitationsAsync(Guid teamId)
            => MockTeamInvitations.GetTeamInvitationsByTeamId(teamId);

        public async Task<List<RequestToTeam>> GetTeamRequestsToTeamAsync(Guid teamId)
            => MockRequestsToTeam.GetRequestToTeamsByTeamId(teamId);

        public async Task<List<RequestTeamToIdea>> GetRequestsTeamToIdeasAsync(Guid teamId)
            => MockRequestTeamToIdeas.GetRequestsTeamToIdeas(teamId);

        public async Task<List<InvitationTeamToIdea>> GetInvitationsTeamToIdeasAsync(Guid teamId)
            => MockInvitationTeamToIdeas.GetInvitationsTeamToIdeas(teamId);
    }
}
