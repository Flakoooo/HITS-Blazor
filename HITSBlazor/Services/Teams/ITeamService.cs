using HITSBlazor.Models.Common.Responses;
using HITSBlazor.Models.Markets.Entities;
using HITSBlazor.Models.Teams.Entities;
using HITSBlazor.Models.Teams.Enums;
using HITSBlazor.Models.Teams.Requests;
using HITSBlazor.Models.Users.Entities;

namespace HITSBlazor.Services.Teams
{
    public interface ITeamService
    {
        event Action<ICollection<User>>? OnInviteMembersCollected;

        event Action<Team>? OnTeamHasDeleted;
        event Action<TeamMember>? OnTeamMemberHasKicked;

        event Func<bool, Task>? OnNewTeamApplicationsHasCreated;

        event Action<Guid, TeamRequestStatus>? OnTeamInvitationStatusHasChanged;

        event Action<Guid, TeamRequestStatus>? OnRequestToTeamStatusHasChanged;

        event Func<Task>? OnRequestsStatusCreated;
        event Action<Guid, TeamRequestStatus>? OnRequestsStatusUpdated;

        void InvokeInvitationEvent(ICollection<User> users);

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
        Task<ListDataResponse<TeamMember>> GetTeamMembersAsync(
            int page,
            Guid? teamId = null,
            string? searchText = null
        );
        Task<bool> CreateTeamAsync(CreateTeamRequest request);
        Task<bool> UpdateTeamAsync(UpdateTeamRequest request);
        Task<bool> UpdateTeamLeader(Guid teamId, Guid? leaderId = null);
        Task KickMemberAsync(TeamMember member);
        Task DeleteTeamAsync(Team team);


        //приглашения в команду участников
        Task<ListDataResponse<TeamInvitation>> GetTeamInvitationsAsync(
            int page,
            Guid? teamId = null,
            string? searchText = null,
            IEnumerable<TeamRequestStatus>? selectedStatuses = null
        );
        Task CreateNewTeamInvitationsAsync(Guid teamId, IEnumerable<Guid> inviteMemberIds);
        Task UpdateTeamInvitationStatusAsync(Guid teamInvitationId, TeamRequestStatus newStatus);

        //заявки в команду участников
        Task<ListDataResponse<RequestToTeam>> GetTeamRequestsToTeamAsync(
            int page,
            Guid? teamId = null,
            string? searchText = null,
            IEnumerable<TeamRequestStatus>? selectedStatuses = null
        );
        Task<bool> CreateNewRequestToTeam(Guid teamId);
        Task UpdateRequestToTeamStatusAsync(Guid requestToTeamId, TeamRequestStatus newStatus);

        //заявки команды в идею
        Task<ListDataResponse<RequestTeamToIdea>> GetRequestsTeamToIdeasAsync(
            int page, Guid teamId, string? searchText = null
        );

        //приглашения команды в идею
        Task<ListDataResponse<InvitationTeamToIdea>> GetInvitationsTeamToIdeasAsync(
            int page, Guid teamId, string? searchText = null
        );

        Task<RequestTeamToIdea> CreateRequestTeamToIdeaAsync(IdeaMarket ideaMarket, Team team, string letter);

        Task<bool> UpdateRequestTeamToIdeaStatusAsync(Guid requestId, TeamRequestStatus newStatus);
    }
}
