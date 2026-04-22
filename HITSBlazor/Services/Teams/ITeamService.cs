using HITSBlazor.Models.Common.Responses;
using HITSBlazor.Models.Markets.Entities;
using HITSBlazor.Models.Teams.Entities;
using HITSBlazor.Models.Teams.Enums;

namespace HITSBlazor.Services.Teams
{
    public interface ITeamService
    {
        event Func<Task>? OnRequestsStatusCreated;
        event Action<Guid, TeamRequestStatus>? OnRequestsStatusUpdated;

        Task<ListDataResponse<Team>> GetTeamsAsync(
            int page,
            string? searchText = null,
            bool? privacy = null,
            bool? hasActiveProject = null,
            HashSet<Guid>? searchSkillIds = null,
            string? orderBy = null,
            bool? byDescending = null
        );
        Task<List<Team>> GetTeamsByOwnerOrLeaderId(Guid userId);
        Task<Team?> GetTeamByIdAsync(Guid teamId);
        Task<bool> DeleteTeamAsync(Team team);


        Task<List<TeamInvitation>> GetTeamInvitationsAsync(Guid teamId);
        Task<List<RequestToTeam>> GetTeamRequestsToTeamAsync(Guid teamId);
        Task<List<RequestTeamToIdea>> GetRequestsTeamToIdeasAsync(Guid teamId);
        Task<List<InvitationTeamToIdea>> GetInvitationsTeamToIdeasAsync(Guid teamId);

        Task<RequestTeamToIdea> CreateRequestTeamToIdeaAsync(IdeaMarket ideaMarket, Team team, string letter);

        Task<bool> UpdateRequestTeamToIdeaStatusAsync(Guid requestId, TeamRequestStatus newStatus);
    }
}
