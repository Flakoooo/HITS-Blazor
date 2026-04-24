using HITSBlazor.Models.Common.Responses;
using HITSBlazor.Models.Markets.Entities;
using HITSBlazor.Models.Teams.Entities;
using HITSBlazor.Models.Teams.Enums;
using HITSBlazor.Models.Users.Entities;
using HITSBlazor.Utils.Mocks.Teams;

namespace HITSBlazor.Services.Teams
{
    public class MockTeamService(GlobalNotificationService globalNotificationService) : ITeamService
    {
        private readonly GlobalNotificationService _globalNotificationService = globalNotificationService;

        public event Action<ICollection<User>>? OnInviteMembersCollected;
        public event Func<Task>? OnRequestsStatusCreated;
        public event Action<Guid, TeamRequestStatus>? OnRequestsStatusUpdated;     

        public void InvokeInvitationEvent(ICollection<User> users) => OnInviteMembersCollected?.Invoke(users);

        public async Task<ListDataResponse<Team>> GetTeamsAsync(
            int page,
            string? searchText,
            bool? privacy,
            bool? hasActiveProject,
            HashSet<Guid>? searchSkillIds,
            string? orderBy,
            bool? byDescending
        )
        {
            return MockTeams.GetAllTeamsByQueryParams(
                page, 
                searchText: searchText, 
                privacy: privacy,
                hasActiveProject: hasActiveProject,
                searchSkillIds: searchSkillIds,
                orderBy:orderBy,
                byDescending: byDescending
            );
        }

        public async Task<List<Team>> GetTeamsByOwnerOrLeaderId(Guid userId)
            => MockTeams.GetTeamsByOwnerIdOrLeaderId(userId);

        public async Task<Team?> GetTeamByIdAsync(Guid teamId) 
            => MockTeams.GetTeamById(teamId);

        //TODO: сделать удаление команды в UI (через событие)
        public async Task<bool> DeleteTeamAsync(Team team)
        {
            if (!MockTeams.DeleteTeam(team))
            {
                _globalNotificationService.ShowError("Не удалось удалить команду");
                return false;
            }

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

        public async Task<RequestTeamToIdea> CreateRequestTeamToIdeaAsync(IdeaMarket ideaMarket, Team team, string letter)
        {
            var newRequest =  MockRequestTeamToIdeas.CreateNewRequest(ideaMarket, team, letter);

            OnRequestsStatusCreated?.Invoke();

            return newRequest;
        }

        public async Task<bool> UpdateRequestTeamToIdeaStatusAsync(Guid requestId, TeamRequestStatus newStatus)
        {
            var result = MockRequestTeamToIdeas.UpdateStatus(requestId, newStatus);
            if (result) OnRequestsStatusUpdated?.Invoke(requestId, newStatus);

            return result;
        }
    }
}
