using HITSBlazor.Models.Common.Entities;
using HITSBlazor.Models.Teams.Entities;
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

        public async Task<List<Team>> GetTeamsAsync(TeamsFilter filter)
        {
            if (_cachedTeams.Count == 0 || DateTime.UtcNow - _lastRefreshTime > _cacheLifetime)
                await RefreshCacheAsync();

            var query = _cachedTeams.AsEnumerable();

            if (!string.IsNullOrWhiteSpace(filter.SearchText))
                query = query.Where(t => t.Name.Contains(filter.SearchText, StringComparison.CurrentCultureIgnoreCase));

            if (filter.Privacy.HasValue)
                query = query.Where(t => t.Closed == filter.Privacy);
            
            if (filter.Survey.HasValue)
                query = query.Where(t => t.StatusQuest == filter.Survey);
            
            if (filter.HasActiveProject.HasValue)
                query = query.Where(t => t.HasActiveProject == filter.HasActiveProject);
            
            if (filter.SearchSkillIds?.Count > 0)
                query = query.Where(t => t.Skills.Any(s => filter.SearchSkillIds.Contains(s.Id)));

            if (!string.IsNullOrWhiteSpace(filter.OrderBy) && filter.ByDescending.HasValue)
            {
                query = (filter.OrderBy, filter.ByDescending.Value) switch
                {
                    (nameof(Team.Closed), true) => query.OrderByDescending(t => t.Closed),
                    (nameof(Team.Closed), false) => query.OrderBy(t => t.Closed),
                    (nameof(Team.Name), true) => query.OrderByDescending(t => t.Name),
                    (nameof(Team.Name), false) => query.OrderBy(t => t.Name),
                    (nameof(Team.HasActiveProject), true) => query.OrderByDescending(t => t.HasActiveProject),
                    (nameof(Team.HasActiveProject), false) => query.OrderBy(t => t.HasActiveProject),
                    (nameof(Team.MembersCount), true) => query.OrderByDescending(t => t.MembersCount),
                    (nameof(Team.MembersCount), false) => query.OrderBy(t => t.MembersCount),
                    (nameof(Team.CreatedAt), true) => query.OrderByDescending(t => t.CreatedAt),
                    (nameof(Team.CreatedAt), false) => query.OrderBy(t => t.CreatedAt),
                    _ => query
                };
            }

            return [.. query];
        }

        public async Task<Team?> GetTeamByIdAsync(Guid teamId) 
            => MockTeams.GetTeamById(teamId);

        public async Task<bool> DeleteTeamAsync(Team team)
        {
            if (!MockTeams.DeleteTeam(team))
            {
                _globalNotificationService.ShowError("Не удалось удалить команду");
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
