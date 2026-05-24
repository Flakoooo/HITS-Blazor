using HITSBlazor.Models.Common.Responses;
using HITSBlazor.Models.Markets.Entities;
using HITSBlazor.Models.Teams.Entities;
using HITSBlazor.Models.Teams.Enums;
using HITSBlazor.Models.Teams.Requests;
using HITSBlazor.Models.Users.Entities;
using HITSBlazor.Services.Auth;
using HITSBlazor.Utils.Mocks.Teams;

namespace HITSBlazor.Services.Teams
{
    public class MockTeamService(
        IAuthService authService, 
        GlobalNotificationService globalNotificationService
    ) : ITeamService
    {
        private readonly IAuthService _authService = authService;
        private readonly GlobalNotificationService _globalNotificationService = globalNotificationService;

        public event Action<ICollection<User>>? OnInviteMembersCollected;

        public event Action<Guid, TeamMember?>? OnTeamLeaderHasChanged;
        public event Action<TeamMember>? OnTeamMemberHasKicked;
        public event Action<Team>? OnTeamHasDeleted;

        public event Func<bool, Task>? OnNewTeamInvitationsHasCreated;
        public event Action<Guid, TeamRequestStatus>? OnTeamInvitationStatusHasChanged;

        public event Func<bool, Task>? OnNewRequestInTeamHasCreated;
        public event Action<Guid, TeamRequestStatus>? OnRequestToTeamStatusHasChanged;

        public event Func<Task>? OnRequestsStatusCreated;
        public event Action<Guid, TeamRequestStatus>? OnRequestsStatusUpdated;

        public event Func<Task>? OnNewInvitationTeamInIdeaCreated;
        public event Action<Guid, TeamRequestStatus>? OnInvitationTeamInIdeaStatusUpdated;

        public void InvokeInvitationEvent(ICollection<User> users) => OnInviteMembersCollected?.Invoke(users);

        public async Task<ListDataResponse<Team>> GetTeamsAsync(
            int page,
            Guid? userId,
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
                userId: userId,
                searchText: searchText, 
                privacy: privacy,
                hasActiveProject: hasActiveProject,
                searchSkillIds: searchSkillIds,
                orderBy:orderBy,
                byDescending: byDescending
            );
        }

        public async Task<Team?> GetTeamByIdAsync(Guid teamId) 
            => MockTeams.GetTeamById(teamId);

        public async Task<ListDataResponse<TeamMember>> GetTeamMembersAsync(
            int page,
            Guid? teamId,
            string? searchText
        ) => MockTeams.GetTeamMembers(page, teamId: teamId, searchText: searchText);

        public async Task<bool> CreateTeamAsync(CreateTeamRequest request)
        {
            var result = MockTeams.CreateTeam(request);
            if (result)
                _globalNotificationService.ShowSuccess($"Команда {request.Name} успешно создана!");
            else
                _globalNotificationService.ShowError($"Не удалось создать команду {request.Name}");

            return result;
        }

        public async Task<bool> UpdateTeamAsync(UpdateTeamRequest request)
        {
            var result = MockTeams.UpdateTeam(request);
            if (result)
                _globalNotificationService.ShowSuccess($"Команда {request.Name} успешно изменена!");
            else
                _globalNotificationService.ShowError($"Не удалось изменить команду {request.Name}");

            return result;
        }

        public async Task<bool> UpdateTeamLeader(Team team, TeamMember? newLeader)
        {
            if (!MockTeams.UpdateTeamLeader(team.Id, newLeader?.Id))
            {
                _globalNotificationService.ShowError("Не удалось изменить Тим-лидера");
                return false;
            }

            OnTeamLeaderHasChanged?.Invoke(team.Id, newLeader ?? team.Owner);
            _globalNotificationService.ShowSuccess($"Тим-лидер успешн изменен!");
            return true;
        }
        
        public async Task KickMemberAsync(TeamMember member)
        {
            if (!MockTeams.KickMember(member))
            {
                _globalNotificationService.ShowError("Не удалось исключить участника");
                return;
            }

            OnTeamMemberHasKicked?.Invoke(member);
        }

        public async Task DeleteTeamAsync(Team team)
        {
            if (!MockTeams.DeleteTeam(team))
            {
                _globalNotificationService.ShowError("Не удалось удалить команду");
                return;
            }

            OnTeamHasDeleted?.Invoke(team);
        }

        //приглашения в команду участников
        public async Task<ListDataResponse<TeamInvitation>> GetTeamInvitationsAsync(
            int page,
            Guid? teamId,
            string? searchText,
            IEnumerable<TeamRequestStatus>? selectedStatuses
        ) => MockTeamInvitations.GetTeamInvitations(
            page, 
            teamId: teamId, 
            searchText: searchText, 
            selectedStatuses: selectedStatuses?.ToHashSet()
        );

        public async Task CreateNewTeamInvitationsAsync(Guid teamId, IEnumerable<Guid> inviteMemberIds)
        {
            MockTeamInvitations.CreateNewInvitations(teamId, inviteMemberIds);

            _globalNotificationService.ShowSuccess("Приглашения отправлены!");
            OnNewTeamInvitationsHasCreated?.Invoke(false);
        }

        public async Task UpdateTeamInvitationStatusAsync(Guid teamInvitationId, TeamRequestStatus newStatus)
        {
            if (!MockTeamInvitations.UpdateTeamInvitationStatus(teamInvitationId, newStatus))
            {
                var errorText = newStatus switch
                {
                    TeamRequestStatus.Withdrawn => "Не удалось отозвать приглашение",
                    TeamRequestStatus.Accepted => "Не удалось принять приглашение",
                    TeamRequestStatus.Canceled => "Ну удалось отклонить приглашение",
                    _ => "Не удалось сменить статус приглашения"
                };

                _globalNotificationService.ShowError(errorText);
                return;
            }

            var successText = newStatus switch
            {
                TeamRequestStatus.Withdrawn => "Приглашение отозвано",
                TeamRequestStatus.Accepted => "Приглашение принято",
                TeamRequestStatus.Canceled => "Приглашение отклонено",
                _ => "Статус приглашения успешно изменен!"
            };

            _globalNotificationService.ShowSuccess(successText);
            OnTeamInvitationStatusHasChanged?.Invoke(teamInvitationId, newStatus);

            return;
        }

        //заявки в команду участников
        public async Task<ListDataResponse<RequestToTeam>> GetTeamRequestsToTeamAsync(
            int page,
            Guid? teamId,
            string? searchText,
            IEnumerable<TeamRequestStatus>? selectedStatuses
        ) => MockRequestsToTeam.GetRequestToTeams(
            page,
            teamId: teamId,
            searchText: searchText,
            selectedStatuses: selectedStatuses?.ToHashSet()
        );

        public async Task<bool> CurrentUserCanSendRequestInTeamAsync(Guid teamId)
        {
            var currentUser = _authService.CurrentUser;
            if (currentUser is null) return false;

            return MockRequestsToTeam.CheckAllowSendRequestInTeam(teamId, currentUser.Id);
        }

        public async Task<bool> CreateNewRequestToTeam(Guid teamId)
        {
            var currentUser = _authService.CurrentUser;
            if (currentUser is null) return false;

            if (!MockRequestsToTeam.CreateNewRequest(teamId, currentUser.Id))
            {
                _globalNotificationService.ShowSuccess("Не удалось отправить заявку");
                return false;
            }

            _globalNotificationService.ShowSuccess("Заявка успешно отправлена!");
            OnNewRequestInTeamHasCreated?.Invoke(false);
            return true;
        }

        public async Task UpdateRequestToTeamStatusAsync(Guid requestToTeamId, TeamRequestStatus newStatus)
        {
            if (!MockRequestsToTeam.UpdateRequestStatus(requestToTeamId, newStatus))
            {
                var errorText = newStatus switch
                {
                    TeamRequestStatus.Withdrawn => "Не удалось отозвать заявку",
                    TeamRequestStatus.Accepted => "Не удалось одобрить заявку",
                    TeamRequestStatus.Canceled => "Ну удалось отклонить заявку",
                    _ => "Не удалось сменить статус приглашения"
                };

                _globalNotificationService.ShowError(errorText);
                return;
            }

            var successText = newStatus switch
            {
                TeamRequestStatus.Withdrawn => "Заявка отозвана",
                TeamRequestStatus.Accepted => "Заявка одобрена",
                TeamRequestStatus.Canceled => "Заявка отклонена",
                _ => "Статус заявки успешно изменен!"
            };

            _globalNotificationService.ShowSuccess(successText);
            OnRequestToTeamStatusHasChanged?.Invoke(requestToTeamId, newStatus);

            return;
        }

        //заявки команды в идею
        public async Task<ListDataResponse<RequestTeamToIdea>> GetRequestsTeamToIdeasAsync(
            int page, 
            Guid teamId, 
            string? searchText
        ) => MockRequestTeamToIdeas.GetRequestsTeamToIdeas(
            page, teamId: teamId, searchText: searchText
        );

        public async Task<RequestTeamToIdea> CreateRequestTeamToIdeaAsync(IdeaMarket ideaMarket, Team team, string letter)
        {
            var newRequest = MockRequestTeamToIdeas.CreateNewRequest(ideaMarket, team, letter);

            OnRequestsStatusCreated?.Invoke();

            return newRequest;
        }

        public async Task<bool> UpdateRequestTeamToIdeaStatusAsync(Guid requestId, TeamRequestStatus newStatus)
        {
            var result = MockRequestTeamToIdeas.UpdateStatus(requestId, newStatus);
            if (!result)
            {
                var errorText = newStatus switch
                {
                    TeamRequestStatus.Withdrawn => "Не удалось отозвать заявку",
                    TeamRequestStatus.Accepted => "Не удалось одобрить заявку",
                    TeamRequestStatus.Canceled => "Ну удалось отклонить заявку",
                    _ => "Не удалось сменить статус заявки"
                };

                _globalNotificationService.ShowError(errorText);
                return false;
            }

            var successText = newStatus switch
            {
                TeamRequestStatus.Withdrawn => "Заявка отозвана",
                TeamRequestStatus.Accepted => "Заявка одобрена",
                TeamRequestStatus.Canceled => "Заявка отклонена",
                _ => "Статус заявки успешно изменен!"
            };

            _globalNotificationService.ShowSuccess(successText);
            OnRequestsStatusUpdated?.Invoke(requestId, newStatus);

            return true;
        }

        //приглашения команды в идею
        public async Task<ListDataResponse<InvitationTeamToIdea>> GetInvitationsTeamToIdeasAsync(
            int page, Guid teamId, string? searchText
        ) => MockInvitationTeamToIdeas.GetInvitationsTeamToIdeas(
            page, teamId: teamId, searchText: searchText
        );

        public async Task<bool> UpdateInvitationTeamToIdeaStatusAsync(Guid invitationId, TeamRequestStatus newStatus)
        {
            var result = MockInvitationTeamToIdeas.UpdateStatus(invitationId, newStatus);
            if (!result)
            {
                var errorText = newStatus switch
                {
                    TeamRequestStatus.Withdrawn => "Не удалось отозвать приглашение",
                    TeamRequestStatus.Accepted => "Не удалось принять приглашение",
                    TeamRequestStatus.Canceled => "Ну удалось отклонить приглашение",
                    _ => "Не удалось сменить статус приглашения"
                };

                _globalNotificationService.ShowError(errorText);
                return false;
            }

            var successText = newStatus switch
            {
                TeamRequestStatus.Withdrawn => "Приглашение отозвано",
                TeamRequestStatus.Accepted => "Приглашение принято",
                TeamRequestStatus.Canceled => "Приглашение отклонено",
                _ => "Статус приглашения успешно изменен!"
            };

            _globalNotificationService.ShowSuccess(successText);
            OnRequestsStatusUpdated?.Invoke(invitationId, newStatus);

            return true;
        }
    }
}
