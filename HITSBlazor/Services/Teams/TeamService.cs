using HITSBlazor.Models.Common.Responses;
using HITSBlazor.Models.Markets.Entities;
using HITSBlazor.Models.Quests.Entities;
using HITSBlazor.Models.Teams.Entities;
using HITSBlazor.Models.Teams.Enums;
using HITSBlazor.Models.Teams.Requests;
using HITSBlazor.Models.Users.Entities;
using HITSBlazor.Services.Auth;
using HITSBlazor.Services.Users;
using HITSBlazor.Utils.Mocks.Teams;
using static HITSBlazor.Utils.Mocks.Common.MockInvitation;

namespace HITSBlazor.Services.Teams
{
    public class TeamService(
        IAuthService authService, 
        TeamApi teamApi,
        ILogger<TeamService> logger,
        GlobalNotificationService globalNotificationService
    ) : ITeamService
    {
        private readonly IAuthService _authService = authService;
        private readonly TeamApi _teamApi = teamApi;
        private readonly ILogger<TeamService> _logger = logger;
        private readonly GlobalNotificationService _globalNotificationService = globalNotificationService;

        public event Action<ICollection<User>>? OnInviteMembersCollected;

        public event Action<Guid, TeamMember?>? OnTeamLeaderHasChanged;
        public event Action<TeamMember>? OnTeamMemberHasKicked;
        public event Action<Team>? OnTeamHasDeleted;

        public event Func<bool, Task>? OnNewTeamInvitationsHasCreated;
        public event Action<Guid, TeamRequestStatus>? OnTeamInvitationStatusHasChanged;

        public event Func<bool, Task>? OnNewRequestInTeamHasCreated;
        public event Action<Guid, TeamRequestStatus>? OnRequestToTeamStatusHasChanged;

        public event Func<bool, Task>? OnNewRequestsTeamInIdeaCreated;
        public event Func<Guid, TeamRequestStatus, Task>? OnRequestTeamInIdeaStatusUpdated;

        public event Func<bool, Task>? OnNewInvitationTeamInIdeaCreated;
        public event Func<Guid, TeamRequestStatus, Task>? OnInvitationTeamInIdeaStatusUpdated;

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
            var result = await _teamApi.GetTeamsAsync(
                page,
                userId: userId,
                searchText: searchText,
                privacy: privacy,
                hasActiveProject: hasActiveProject,
                searchSkillIds: searchSkillIds,
                orderBy: orderBy,
                byDescending: byDescending
            );

            if (result.IsSuccess && result.Response is not null)
                return result.Response;

            if (!string.IsNullOrWhiteSpace(result.Message))
            {
                _globalNotificationService.ShowError(result.Message);
                if (_logger.IsEnabled(LogLevel.Warning))
                    _logger.LogWarning("Get teams failed: {Error}", result.Message);
            }

            return new ListDataResponse<Team>(0, []);
        }

        public async Task<Team?> GetTeamByIdAsync(Guid teamId)
        {
            var result = await _teamApi.GetTeamAsync(teamId);

            if (!string.IsNullOrWhiteSpace(result.Message))
            {
                _globalNotificationService.ShowError(result.Message);
                if (_logger.IsEnabled(LogLevel.Warning))
                    _logger.LogWarning("Get team failed: {Error}", result.Message);
            }

            return result.Response;
        }

        public async Task<bool> CreateTeamAsync(CreateTeamRequest request)
        {
            var result = await _teamApi.CreateTeamAsync(request);
            if (result.IsSuccess && result.Response is not null)
            {
                _globalNotificationService.ShowSuccess($"Команда {request.Name} успешно создана!");
                return true;
            }

            if (!string.IsNullOrWhiteSpace(result.Message))
            {
                _globalNotificationService.ShowError(result.Message);
                if (_logger.IsEnabled(LogLevel.Warning))
                    _logger.LogWarning("Create team failed: {Error}", result.Message);
            }

            return false;
        }

        public async Task<bool> UpdateTeamAsync(UpdateTeamRequest request)
        {
            var result = await _teamApi.UpdateTeamAsync(request);
            if (result.IsSuccess && result.Response is not null)
            {
                _globalNotificationService.ShowSuccess($"Команда {request.Name} успешно изменена!");
                return true;
            }

            if (!string.IsNullOrWhiteSpace(result.Message))
            {
                _globalNotificationService.ShowError(result.Message);
                if (_logger.IsEnabled(LogLevel.Warning))
                    _logger.LogWarning("Update team failed: {Error}", result.Message);
            }

            return false;
        }

        public async Task<bool> UpdateTeamLeader(Team team, TeamMember? newLeader)
        {
            var newTeamLeader = newLeader ?? team.Owner;
            var result = await _teamApi.UpdateTeamLeaderAsync(team.Id, newTeamLeader.UserId);
            if (result.IsSuccess && result.Response is not null)
            {
                OnTeamLeaderHasChanged?.Invoke(team.Id, newTeamLeader);
                _globalNotificationService.ShowSuccess(result.Response);
                return true;
            }

            _globalNotificationService.ShowError("Не удалось обновить тим-лидера");
            if (_logger.IsEnabled(LogLevel.Warning))
                _logger.LogWarning("Update team leader failed: {Error}", "Не удалось обновить тим-лидера");

            return false;
        }
        
        public async Task KickMemberAsync(TeamMember member)
        {
            var result = await _teamApi.KickMemberAsync(member.TeamId, member.UserId);
            if (result.IsSuccess && result.Response is not null)
            {
                OnTeamMemberHasKicked?.Invoke(member);
                _globalNotificationService.ShowSuccess(result.Response);
            }
            else
            {
                _globalNotificationService.ShowError("Не удалось исключить пользователя");
                if (_logger.IsEnabled(LogLevel.Warning))
                    _logger.LogWarning("Kick member failed: {Error}", "Не удалось исключить пользователя");
            }
        }

        public async Task LeaveFromTeamAsync(TeamMember member)
        {
            var result = await _teamApi.LeaveFromTeamAsync(member.TeamId);
            if (result.IsSuccess && result.Response is not null)
            {
                OnTeamMemberHasKicked?.Invoke(member);
                _globalNotificationService.ShowSuccess(result.Response);
            }
            else
            {
                _globalNotificationService.ShowError("Не удалось покинуть команду");
                if (_logger.IsEnabled(LogLevel.Warning))
                    _logger.LogWarning("Leave from team failed: {Error}", "Не удалось покинуть команду");
            }
        }

        public async Task DeleteTeamAsync(Team team)
        {
            var result = await _teamApi.DeleteTeamAsync(team.Id);
            if (result.IsSuccess && result.Response is not null)
            {
                OnTeamHasDeleted?.Invoke(team);
                _globalNotificationService.ShowSuccess(result.Response);
            }
            else
            {
                _globalNotificationService.ShowError("Не удалось удалить команду");
                if (_logger.IsEnabled(LogLevel.Warning))
                    _logger.LogWarning("Delete team failed: {Error}", "Не удалось удалить команду");
            }
        }

        //приглашения в команду участников
        public async Task<ListDataResponse<TeamInvitation>> GetTeamInvitationsAsync(
            int page,
            Guid? teamId,
            string? searchText,
            IEnumerable<TeamRequestStatus>? selectedStatuses
        )
        {
            var result = await _teamApi.GetTeamInvitationsAsync(
                page,
                teamId: teamId,
                searchText: searchText,
                selectedStatuses: selectedStatuses
            );

            if (result.IsSuccess && result.Response is not null)
                return result.Response;

            if (!string.IsNullOrWhiteSpace(result.Message))
            {
                _globalNotificationService.ShowError(result.Message);
                if (_logger.IsEnabled(LogLevel.Warning))
                    _logger.LogWarning("Get team invitatons failed: {Error}", result.Message);
            }

            return new ListDataResponse<TeamInvitation>(0, []);
        }

        public async Task CreateNewTeamInvitationsAsync(Guid teamId, IEnumerable<User> invitedMembers)
        {
            var currentUser = _authService.CurrentUser;
            if (currentUser is null)
            {
                _globalNotificationService.ShowError("Не удалось отправить приглашения");
                return;
            }

            var invitations = new List<CreateTeamInvitation>();
            foreach (var member in invitedMembers)
            {
                invitations.Add(new CreateTeamInvitation 
                { 
                    UserId = member.Id, 
                    TeamId = teamId, 
                    Email = member.Email, 
                    FirstName = currentUser.FirstName, 
                    LastName = currentUser.LastName 
                });
            }

            var result = await _teamApi.SendTeamInvitationsAsync(invitations);
            if (result.IsSuccess && result.Response is not null)
            {
                OnNewTeamInvitationsHasCreated?.Invoke(false);
                _globalNotificationService.ShowSuccess(result.Response);
            }
            else if (!string.IsNullOrWhiteSpace(result.Message))
            {
                _globalNotificationService.ShowError(result.Message);
                if (_logger.IsEnabled(LogLevel.Warning))
                    _logger.LogWarning("Create team invitations failed: {Error}", result.Message);
            }
        }

        public async Task UpdateTeamInvitationStatusAsync(Guid teamInvitationId, TeamRequestStatus newStatus)
        {
            var result = await _teamApi.UpdateTeamInvitationStatusAsync(teamInvitationId, newStatus);
            if (result.IsSuccess && result.Response is not null)
            {
                var successText = newStatus switch
                {
                    TeamRequestStatus.Withdrawn => "Приглашение отозвано",
                    TeamRequestStatus.Accepted => "Приглашение принято",
                    TeamRequestStatus.Canceled => "Приглашение отклонено",
                    _ => "Статус приглашения успешно изменен!"
                };

                OnTeamInvitationStatusHasChanged?.Invoke(teamInvitationId, newStatus);
                _globalNotificationService.ShowSuccess(successText);
            }
            else
            {
                var errorText = newStatus switch
                {
                    TeamRequestStatus.Withdrawn => "Не удалось отозвать приглашение",
                    TeamRequestStatus.Accepted => "Не удалось принять приглашение",
                    TeamRequestStatus.Canceled => "Ну удалось отклонить приглашение",
                    _ => "Не удалось сменить статус приглашения"
                };

                _globalNotificationService.ShowError(errorText);
                if (_logger.IsEnabled(LogLevel.Warning))
                    _logger.LogWarning("Update team invitation status failed: {Error}", errorText);
            }
        }

        //заявки в команду участников
        public async Task<ListDataResponse<RequestToTeam>> GetTeamRequestsToTeamAsync(
            int page,
            Guid? teamId,
            string? searchText,
            IEnumerable<TeamRequestStatus>? selectedStatuses
        )
        {
            var result = await _teamApi.GetTeamRequestsAsync(
                page,
                teamId: teamId,
                searchText: searchText,
                selectedStatuses: selectedStatuses
            );

            if (result.IsSuccess && result.Response is not null)
                return result.Response;

            if (!string.IsNullOrWhiteSpace(result.Message))
            {
                _globalNotificationService.ShowError(result.Message);
                if (_logger.IsEnabled(LogLevel.Warning))
                    _logger.LogWarning("Get team requests failed: {Error}", result.Message);
            }

            return new ListDataResponse<RequestToTeam>(0, []);
        }

        public async Task<bool> CurrentUserCanSendRequestInTeamAsync(Guid teamId)
        {
            var currentUser = _authService.CurrentUser;
            if (currentUser is null) return false;

            return MockRequestsToTeam.CheckAllowSendRequestInTeam(teamId, currentUser.Id);
        }

        public async Task<bool> CreateNewRequestToTeam(Guid teamId)
        {
            var result = await _teamApi.SendTeamRequestAsync(teamId);
            if (result.IsSuccess && result.Response is not null)
            {
                OnNewRequestInTeamHasCreated?.Invoke(false);
                _globalNotificationService.ShowSuccess(result.Response);
                return true;
            }

            _globalNotificationService.ShowError("Не удалось отправить заявку");
            if (_logger.IsEnabled(LogLevel.Warning))
                _logger.LogWarning("Create team invitations failed: {Error}", "Не удалось отправить заявку");
            return false;
        }

        public async Task UpdateRequestToTeamStatusAsync(Guid requestToTeamId, TeamRequestStatus newStatus)
        {
            var result = await _teamApi.UpdateTeamRequestStatusAsync(requestToTeamId, newStatus);
            if (result.IsSuccess && result.Response is not null)
            {
                var successText = newStatus switch
                {
                    TeamRequestStatus.Withdrawn => "Заявка отозвана",
                    TeamRequestStatus.Accepted => "Заявка одобрена",
                    TeamRequestStatus.Canceled => "Заявка отклонена",
                    _ => "Статус заявки успешно изменен!"
                };

                OnRequestToTeamStatusHasChanged?.Invoke(requestToTeamId, newStatus);
                _globalNotificationService.ShowSuccess(successText);
            }
            else
            {
                var errorText = newStatus switch
                {
                    TeamRequestStatus.Withdrawn => "Не удалось отозвать заявку",
                    TeamRequestStatus.Accepted => "Не удалось одобрить заявку",
                    TeamRequestStatus.Canceled => "Ну удалось отклонить заявку",
                    _ => "Не удалось сменить статус приглашения"
                };

                _globalNotificationService.ShowError(errorText);
                if (_logger.IsEnabled(LogLevel.Warning))
                    _logger.LogWarning("Update team request status failed: {Error}", errorText);
            }
        }

        //заявки команды в идею
        public async Task<ListDataResponse<RequestTeamToIdea>> GetRequestsTeamToIdeasAsync(
            int page, 
            Guid? teamId, 
            Guid? ideaMarketId,
            string? searchText
        )
        {
            var result = await _teamApi.GetRequestsTeamToIdeasAsync(
                page, 
                teamId: teamId, 
                ideaMarketId: ideaMarketId, 
                searchText: searchText
            );

            if (result.IsSuccess && result.Response is not null)
                return result.Response;

            if (!string.IsNullOrWhiteSpace(result.Message))
            {
                _globalNotificationService.ShowError(result.Message);
                if (_logger.IsEnabled(LogLevel.Warning))
                    _logger.LogWarning("Get team requests to idea failed: {Error}", result.Message);
            }

            return new ListDataResponse<RequestTeamToIdea>(0, []);
        }

        public async Task<List<RequestTeamToIdea>> GetTeamRequestsForCurretnIdeaMarketAndTeamsAsync(
            Guid ideaMarketId,
            IEnumerable<Guid> currentTeamIds
        )
        {
            var result = await _teamApi.GetTeamRequestsForCurretnIdeaMarketAndTeamsAsync(
                ideaMarketId, currentTeamIds.ToHashSet()
            );

            if (result.IsSuccess && result.Response is not null)
                return result.Response.ToList();

            if (!string.IsNullOrWhiteSpace(result.Message))
            {
                _globalNotificationService.ShowError(result.Message);
                if (_logger.IsEnabled(LogLevel.Warning))
                    _logger.LogWarning("Get team requests to current idea failed: {Error}", result.Message);
            }

            return [];
        }

        public async Task<RequestTeamToIdea?> CreateRequestTeamToIdeaAsync(IdeaMarket ideaMarket, Team team, string letter)
        {
            var result = await _teamApi.SendRequestTeamToIdeaAsync(team.Id, ideaMarket.Id, letter);
            if (result.IsSuccess && result.Response is not null)
            {
                OnNewRequestsTeamInIdeaCreated?.Invoke(false);
                _globalNotificationService.ShowSuccess("Заявка успешно отправлена");
            }

            _globalNotificationService.ShowError("Не удалось отправить заявку");
            if (_logger.IsEnabled(LogLevel.Warning))
                _logger.LogWarning("Create team request to idea failed: {Error}", "Не удалось отправить заявку");

            return result.Response;
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
            OnRequestTeamInIdeaStatusUpdated?.Invoke(requestId, newStatus);

            return true;
        }

        //приглашения команды в идею
        public async Task<ListDataResponse<InvitationTeamToIdea>> GetInvitationsTeamToIdeasAsync(
            int page,
            Guid? teamId,
            Guid? ideaMarketId,
            string? searchText
        )
        {
            _globalNotificationService.ShowError("Метод GetInvitationsTeamToIdeasAsync не реализован");
            return new ListDataResponse<InvitationTeamToIdea>(0, []);
        }

        public async Task<List<InvitationTeamToIdea>> GetTeamInvitationForCurrentTeamAndIdeaMarketsAsync(
            Guid teamId,
            IEnumerable<Guid> currentIdeaMarketIds
        )
        {
            _globalNotificationService.ShowError("Метод GetTeamInvitationForCurrentTeamAndIdeaMarketsAsync не реализован");
            return [];
        }

        public async Task<InvitationTeamToIdea?> CreateInvitationTeamToIdeaAsync(IdeaMarket ideaMarket, Team team)
        {
            _globalNotificationService.ShowError("Метод CreateInvitationTeamToIdeaAsync не реализован");
            return null;
        }

        public async Task<bool> UpdateInvitationTeamToIdeaStatusAsync(Guid invitationId, TeamRequestStatus newStatus)
        {
            _globalNotificationService.ShowError("Метод UpdateInvitationTeamToIdeaStatusAsync не реализован");
            return false;
        }
    }
}
