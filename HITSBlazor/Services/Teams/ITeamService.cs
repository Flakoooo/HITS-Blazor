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

        event Action<Guid, TeamMember?>? OnTeamLeaderHasChanged;
        event Action<TeamMember>? OnTeamMemberHasKicked;
        event Action<Team>? OnTeamHasDeleted;

        event Func<bool, Task>? OnNewTeamInvitationsHasCreated;
        event Action<Guid, TeamRequestStatus>? OnTeamInvitationStatusHasChanged;

        event Func<bool, Task>? OnNewRequestInTeamHasCreated;
        event Action<Guid, TeamRequestStatus>? OnRequestToTeamStatusHasChanged;

        event Func<bool, Task>? OnNewRequestsTeamInIdeaCreated;
        event Func<Guid, TeamRequestStatus, Task>? OnRequestTeamInIdeaStatusUpdated;

        event Func<bool, Task>? OnNewInvitationTeamInIdeaCreated;
        event Func<Guid, TeamRequestStatus, Task>? OnInvitationTeamInIdeaStatusUpdated;

        void InvokeInvitationEvent(ICollection<User> users);

        Task<ListDataResponse<Team>> GetTeamsAsync(
            int page,
            Guid? userId = null,
            string? searchText = null,
            bool? privacy = null,
            bool? hasActiveProject = null,
            HashSet<Guid>? searchSkillIds = null,
            string? orderBy = null,
            bool? byDescending = null
        );
        Task<Team?> GetTeamByIdAsync(Guid teamId);
        Task<bool> CreateTeamAsync(CreateTeamRequest request);
        Task<bool> UpdateTeamAsync(UpdateTeamRequest request);
        Task<bool> UpdateTeamLeader(Team team, TeamMember? newLeader);
        Task KickMemberAsync(TeamMember member);
        Task LeaveFromTeamAsync(TeamMember member);
        Task DeleteTeamAsync(Team team);


        //приглашения в команду участников
        Task<ListDataResponse<TeamInvitation>> GetTeamInvitationsAsync(
            int page,
            Guid? teamId = null,
            string? searchText = null,
            IEnumerable<TeamRequestStatus>? selectedStatuses = null
        );
        Task CreateNewTeamInvitationsAsync(Guid teamId, IEnumerable<User> invitedMembers);
        Task UpdateTeamInvitationStatusAsync(Guid teamInvitationId, TeamRequestStatus newStatus);

        //заявки в команду участников
        Task<ListDataResponse<RequestToTeam>> GetTeamRequestsToTeamAsync(
            int page,
            Guid? teamId = null,
            string? searchText = null,
            IEnumerable<TeamRequestStatus>? selectedStatuses = null
        );
        Task<bool> CurrentUserCanSendRequestInTeamAsync(Guid teamId);
        Task<bool> CreateNewRequestToTeam(Guid teamId);
        Task UpdateRequestToTeamStatusAsync(Guid requestToTeamId, TeamRequestStatus newStatus);

        //заявки команды в идею
        Task<ListDataResponse<RequestTeamToIdea>> GetRequestsTeamToIdeasAsync(
            int page,
            Guid? teamId = null,
            Guid? ideaMarketId = null,
            string? searchText = null
        );
        Task<List<RequestTeamToIdea>> GetTeamRequestsForCurretnIdeaMarketAndTeamsAsync(
            Guid ideaMarketId,
            IEnumerable<Guid> currentTeamIds
        );
        Task<RequestTeamToIdea?> CreateRequestTeamToIdeaAsync(IdeaMarket ideaMarket, Team team, string letter);

        Task<bool> UpdateRequestTeamToIdeaStatusAsync(Guid requestId, TeamRequestStatus newStatus);

        //приглашения команды в идею
        Task<ListDataResponse<InvitationTeamToIdea>> GetInvitationsTeamToIdeasAsync(
            int page,
            Guid? teamId = null,
            Guid? ideaMarketId = null,
            string? searchText = null
        );
        Task<List<InvitationTeamToIdea>> GetTeamInvitationForCurrentTeamAndIdeaMarketsAsync(
            Guid teamId,
            IEnumerable<Guid> currentIdeaMarketIds
        );
        Task<InvitationTeamToIdea?> CreateInvitationTeamToIdeaAsync(IdeaMarket ideaMarket, Team team);
        Task<bool> UpdateInvitationTeamToIdeaStatusAsync(Guid invitationId, TeamRequestStatus newStatus);
    }
}
