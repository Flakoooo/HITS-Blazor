using HITSBlazor.Models.Markets.Entities;
using HITSBlazor.Models.Teams.Entities;
using HITSBlazor.Models.Teams.Enums;

namespace HITSBlazor.Services.Teams
{
    public interface ITeamService
    {
        event Action<Guid, TeamRequestStatus>? OnRequestsStatusUpdated;

        Task<List<Team>> GetTeamsAsync(TeamsFilter filter);
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
