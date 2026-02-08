using HITSBlazor.Models.Teams.Entities;

namespace HITSBlazor.Services.Teams
{
    public interface ITeamService
    {
        Task<List<Team>> GetTeamsAsync();
        Task<Team?> GetTeamByIdAsync(Guid teamId);
        Task<List<TeamInvitation>> GetTeamInvitationsAsync(Guid teamId);
        Task<List<RequestToTeam>> GetTeamRequestsToTeamAsync(Guid teamId);
        Task<List<RequestTeamToIdea>> GetRequestsTeamToIdeasAsync(Guid teamId);
        Task<List<InvitationTeamToIdea>> GetInvitationsTeamToIdeasAsync(Guid teamId);
    }
}
