using HITSBlazor.Models.Teams.Entities;
using HITSBlazor.Utils.Mocks.Teams;

namespace HITSBlazor.Services.Teams
{
    public class MockTeamService : ITeamService
    {
        public async Task<List<Team>> GetTeamsAsync() 
            => MockTeams.GetAllTeams();

        public async Task<Team?> GetTeamByIdAsync(Guid teamId) 
            => MockTeams.GetTeamById(teamId);

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
