using ApexCharts;
using HITSBlazor.Components.Tags;
using HITSBlazor.Models.Common.Responses;
using HITSBlazor.Models.Teams.Entities;
using HITSBlazor.Models.Teams.Enums;
using HITSBlazor.Models.Teams.Requests;
using HITSBlazor.Utils;
using System.Net.Http.Json;
using static HITSBlazor.Utils.Mocks.Common.MockInvitation;

namespace HITSBlazor.Services.Teams
{
    public class TeamApi(
        IHttpClientFactory httpClientFactory,
        ILogger<TeamApi> logger
    ) : BaseApiService(httpClientFactory.CreateClient(Settings.HttpClientName), logger)
    {
        private readonly string _teamPath = "/api/team";

        private const string GET_TEAMS_OPERATION = "GetTeams";
        private const string GET_TEAM_OPERATION = "GetTeam";
        private const string CREATE_TEAM_OPERATION = "CreateTeam";
        private const string UPDATE_TEAM_OPERATION = "UpdateTeam";
        private const string UPDATE_TEAM_LEADER_OPERATION = "UpdateTeamLeader";
        private const string KICK_MEMBER_OPERATION = "KickMember";
        private const string LEAVE_FROM_TEAM_OPERATION = "LeaveFromTeam";
        private const string DELETE_TEAM_OPERATION = "DeleteTeam";

        private const string GET_TEAM_INVITATIONS = "GetTeamInvitations";
        private const string SEND_TEAM_INVITATION = "SendTeamInvitation";
        private const string UPDATE_TEAM_INVITATION_STATUS = "UpdateTeamInvitationStatus";

        private const string GET_TEAM_REQUESTS = "GetTeamRequest";
        private const string SEND_TEAM_REQUEST = "SendTeamRequest";
        private const string SEND_TEAM_REQUEST_ALLOWED = "SendTeamRequestAllowed";
        private const string UPDATE_TEAM_REQUEST_STATUS = "UpdateTeamRequestStatus";

        private const string GET_MARKET_TEAM_REQUESTS = "GetMarketTeamRequest";
        private const string GET_MARKET_TEAM_REQUESTS_IN_IDEA = "GetMarketTeamRequestInIdea";
        private const string SEND_MARKET_TEAM_REQUEST = "SendMarketTeamRequest";
        private const string UPDATE_MARKET_TEAM_REQUEST_STATUS = "UpdateMarketTeamRequestStatus";


        //TEAMS
        public async Task<ServiceResponse<ListDataResponse<Team>>> GetTeamsAsync(
            int page,
            int pageSize = 20,
            Guid? userId = null,
            string? searchText = null,
            bool? privacy = null,
            bool? hasActiveProject = null,
            HashSet<Guid>? searchSkillIds = null,
            string? orderBy = null,
            bool? byDescending = null
        )
        {
            string path = $"{_teamPath}{AddQuery("page", page, false)}{AddQuery("page_size", pageSize)}";

            if (userId.HasValue)
                path += AddQuery("user_id", userId.Value);

            if (!string.IsNullOrWhiteSpace(searchText))
                path += AddQuery("search_text", searchText);

            if (privacy.HasValue)
                path += AddQuery("privacy", privacy.Value);

            if (hasActiveProject.HasValue)
                path += AddQuery("has_active_project", hasActiveProject.Value);

            if (searchSkillIds?.Count > 0)
                path += AddQuery("search_skill_id", searchSkillIds);

            if (!string.IsNullOrWhiteSpace(orderBy))
                path += AddQuery("order_by", orderBy);

            if (byDescending.HasValue)
                path += AddQuery("by_descending", byDescending.Value);

            return await ExecuteApiCallAsync(
                apiCall: () => _httpClient.GetAsync(path),
                successHandler: async response =>
                {
                    var teams = await response.Content.ReadFromJsonAsync<ListDataResponse<Team>>(Settings.BaseJsonOptions);
                    if (teams is null)
                    {
                        LogFail(GET_TEAMS_OPERATION, response.StatusCode, "Error when parse teams");

                        return ServiceResponse<ListDataResponse<Team>>.Failure("Не удалось получить команды", response.StatusCode);
                    }

                    return ServiceResponse<ListDataResponse<Team>>.Success(teams);
                },
                operationName: GET_TEAMS_OPERATION
            );
        }

        public async Task<ServiceResponse<Team?>> GetTeamAsync(Guid teamId)
        {
            string path = $"{_teamPath}/{teamId}";

            return await ExecuteApiCallAsync(
                apiCall: () => _httpClient.GetAsync(path),
                successHandler: async response =>
                {
                    var team = await response.Content.ReadFromJsonAsync<Team>(Settings.BaseJsonOptions);
                    if (team is null)
                    {
                        LogFail(GET_TEAM_OPERATION, response.StatusCode, "Error when parse team");

                        return ServiceResponse<Team?>.Failure("Не удалось получить команду", response.StatusCode);
                    }

                    return ServiceResponse<Team?>.Success(team);
                },
                operationName: GET_TEAM_OPERATION
            );
        }

        public async Task<ServiceResponse<Team?>> CreateTeamAsync(CreateTeamRequest request)
        {
            var content = SerializeData(request);

            return await ExecuteApiCallAsync(
                apiCall: () => _httpClient.PostAsync(_teamPath, content),
                successHandler: async response =>
                {
                    var team = await response.Content.ReadFromJsonAsync<Team>(Settings.BaseJsonOptions);
                    if (team is null)
                    {
                        LogFail(CREATE_TEAM_OPERATION, response.StatusCode, "Error when create team");

                        return ServiceResponse<Team?>.Failure("Не удалось создать команду", response.StatusCode);
                    }

                    return ServiceResponse<Team?>.Success(team);
                },
                operationName: CREATE_TEAM_OPERATION
            );
        }

        public async Task<ServiceResponse<Team?>> UpdateTeamAsync(UpdateTeamRequest request)
        {
            var content = SerializeData(request);

            return await ExecuteApiCallAsync(
                apiCall: () => _httpClient.PutAsync(_teamPath, content),
                successHandler: async response =>
                {
                    var team = await response.Content.ReadFromJsonAsync<Team>(Settings.BaseJsonOptions);
                    if (team is null)
                    {
                        LogFail(UPDATE_TEAM_OPERATION, response.StatusCode, "Error when update team");

                        return ServiceResponse<Team?>.Failure("Не удалось обновить команду", response.StatusCode);
                    }

                    return ServiceResponse<Team?>.Success(team);
                },
                operationName: UPDATE_TEAM_OPERATION
            );
        }

        public async Task<ServiceResponse<string>> UpdateTeamLeaderAsync(Guid teamId, Guid newLeaderId)
        {
            string path = $"{_teamPath}/leader/{teamId}/{newLeaderId}";

            return await ExecuteApiCallAsync(
                apiCall: () => _httpClient.PutAsync(_teamPath, null),
                successHandler: async response => ServiceResponse<string>.Success("Тим-лидер успешно изменен"),
                operationName: UPDATE_TEAM_LEADER_OPERATION
            );
        }

        public async Task<ServiceResponse<string>> KickMemberAsync(Guid teamId, Guid memberId)
        {
            string path = $"{_teamPath}/members/kick/{teamId}/{memberId}";

            return await ExecuteApiCallAsync(
                apiCall: () => _httpClient.DeleteAsync(_teamPath),
                successHandler: async response => ServiceResponse<string>.Success("Пользователь успешно исключен"),
                operationName: KICK_MEMBER_OPERATION
            );
        }

        public async Task<ServiceResponse<string>> LeaveFromTeamAsync(Guid teamId)
        {
            string path = $"{_teamPath}/leave/{teamId}";

            return await ExecuteApiCallAsync(
                apiCall: () => _httpClient.DeleteAsync(_teamPath),
                successHandler: async response => ServiceResponse<string>.Success("Вы покинули команду"),
                operationName: LEAVE_FROM_TEAM_OPERATION
            );
        }

        public async Task<ServiceResponse<string>> DeleteTeamAsync(Guid teamId)
        {
            string path = $"{_teamPath}/{teamId}";

            return await ExecuteApiCallAsync(
                apiCall: () => _httpClient.DeleteAsync(_teamPath),
                successHandler: async response => ServiceResponse<string>.Success("Команда удалена"),
                operationName: DELETE_TEAM_OPERATION
            );
        }

        //TEAMS INVITATIONS
        public async Task<ServiceResponse<ListDataResponse<TeamInvitation>>> GetTeamInvitationsAsync(
            int page,
            int pageSize = 20,
            Guid? teamId = null,
            string? searchText= null,
            IEnumerable<TeamRequestStatus>? selectedStatuses = null
        )
        {
            string path = $"{_teamPath}/invitations{AddQuery("page", page, false)}{AddQuery("page_size", pageSize)}";

            if (teamId.HasValue)
                path += AddQuery("team_id", teamId.Value);

            if (!string.IsNullOrWhiteSpace(searchText))
                path += AddQuery("search_text", searchText);

            if (selectedStatuses is not null && selectedStatuses.Any())
                path += AddQuery("selected_status", selectedStatuses);

            return await ExecuteApiCallAsync(
                apiCall: () => _httpClient.GetAsync(path),
                successHandler: async response =>
                {
                    var invitations = await response.Content.ReadFromJsonAsync<ListDataResponse<TeamInvitation>>(Settings.BaseJsonOptions);
                    if (invitations is null)
                    {
                        LogFail(GET_TEAMS_OPERATION, response.StatusCode, "Error when parse team invitations");

                        return ServiceResponse<ListDataResponse<TeamInvitation>>.Failure("Не удалось получить приглашения", response.StatusCode);
                    }

                    return ServiceResponse<ListDataResponse<TeamInvitation>>.Success(invitations);
                },
                operationName: GET_TEAM_INVITATIONS
            );
        }

        public async Task<ServiceResponse<string>> SendTeamInvitationsAsync(IEnumerable<CreateTeamInvitation> invitations)
        {
            string path = $"{_teamPath}/invitations";
            var content = SerializeData(invitations);

            return await ExecuteApiCallAsync(
                apiCall: () => _httpClient.PostAsync(path, content),
                successHandler: async response => ServiceResponse<string>.Success("Приглашения успешно отправлены!"),
                operationName: SEND_TEAM_INVITATION
            );
        }

        public async Task<ServiceResponse<TeamInvitation>> UpdateTeamInvitationStatusAsync(Guid teamInvitationId, TeamRequestStatus newStatus)
        {
            string path = $"{_teamPath}/invitations/status/{teamInvitationId}/{newStatus}";

            return await ExecuteApiCallAsync(
                apiCall: () => _httpClient.PutAsync(path, null),
                successHandler: async response =>
                {
                    var invitation = await response.Content.ReadFromJsonAsync<TeamInvitation>(Settings.BaseJsonOptions);
                    if (invitation is null)
                    {
                        LogFail(UPDATE_TEAM_INVITATION_STATUS, response.StatusCode, "Error when update team invitation status");

                        return ServiceResponse<TeamInvitation>.Failure("Не удалось изменить статус приглашения", response.StatusCode);
                    }

                    return ServiceResponse<TeamInvitation>.Success(invitation);
                },
                operationName: UPDATE_TEAM_INVITATION_STATUS
            );
        }

        //TEAMS REQUESTS
        public async Task<ServiceResponse<ListDataResponse<RequestToTeam>>> GetTeamRequestsAsync(
            int page,
            int pageSize = 20,
            Guid? teamId = null,
            string? searchText = null,
            IEnumerable<TeamRequestStatus>? selectedStatuses = null
        )
        {
            string path = $"{_teamPath}/requests{AddQuery("page", page, false)}{AddQuery("page_size", pageSize)}";

            if (teamId.HasValue)
                path += AddQuery("team_id", teamId.Value);

            if (!string.IsNullOrWhiteSpace(searchText))
                path += AddQuery("search_text", searchText);

            if (selectedStatuses is not null && selectedStatuses.Any())
                path += AddQuery("selected_status", selectedStatuses);

            return await ExecuteApiCallAsync(
                apiCall: () => _httpClient.GetAsync(path),
                successHandler: async response =>
                {
                    var requests = await response.Content.ReadFromJsonAsync<ListDataResponse<RequestToTeam>>(Settings.BaseJsonOptions);
                    if (requests is null)
                    {
                        LogFail(GET_TEAM_REQUESTS, response.StatusCode, "Error when parse team requests");

                        return ServiceResponse<ListDataResponse<RequestToTeam>>.Failure("Не удалось получить заявки", response.StatusCode);
                    }

                    return ServiceResponse<ListDataResponse<RequestToTeam>>.Success(requests);
                },
                operationName: GET_TEAM_REQUESTS
            );
        }

        public async Task<ServiceResponse<string>> SendTeamRequestAsync(Guid teamId)
        {
            string path = $"{_teamPath}/requests/{teamId}";

            return await ExecuteApiCallAsync(
                apiCall: () => _httpClient.PostAsync(path, null),
                successHandler: async response => ServiceResponse<string>.Success("Заявка успешно отправлена!"),
                operationName: SEND_TEAM_REQUEST
            );
        }

        public async Task<ServiceResponse<bool>> CurrentUserCanSendRequestInTeamAsync(Guid teamId)
        {
            string path = $"{_teamPath}/request/allowed/{teamId}";

            return await ExecuteApiCallAsync(
                apiCall: () => _httpClient.GetAsync(path),
                successHandler: async response => ServiceResponse<bool>.Success(true),
                operationName: SEND_TEAM_REQUEST_ALLOWED
            );
        }

        public async Task<ServiceResponse<RequestToTeam>> UpdateTeamRequestStatusAsync(Guid teamInvitationId, TeamRequestStatus newStatus)
        {
            string path = $"{_teamPath}/requests/status/{teamInvitationId}/{newStatus}";

            return await ExecuteApiCallAsync(
                apiCall: () => _httpClient.PutAsync(path, null),
                successHandler: async response =>
                {
                    var request = await response.Content.ReadFromJsonAsync<RequestToTeam>(Settings.BaseJsonOptions);
                    if (request is null)
                    {
                        LogFail(UPDATE_TEAM_REQUEST_STATUS, response.StatusCode, "Error when update team request status");

                        return ServiceResponse<RequestToTeam>.Failure("Не удалось изменить статус заявки", response.StatusCode);
                    }

                    return ServiceResponse<RequestToTeam>.Success(request);
                },
                operationName: UPDATE_TEAM_REQUEST_STATUS
            );
        }

        //TEAMS IDEAS
        //TEAM REQUESTS
        public async Task<ServiceResponse<ListDataResponse<RequestTeamToIdea>>> GetRequestsTeamToIdeasAsync(
            int page,
            int pageSize = 20,
            Guid? teamId = null,
            Guid? ideaMarketId = null,
            string? searchText = null
        )
        {
            string path = $"{_teamPath}/market/requests{AddQuery("page", page, false)}{AddQuery("page_size", pageSize)}";

            if (teamId.HasValue)
                path += AddQuery("team_id", teamId.Value);

            if (ideaMarketId.HasValue)
                path += AddQuery("idea_market_id", ideaMarketId.Value);

            if (!string.IsNullOrWhiteSpace(searchText))
                path += AddQuery("search_text", searchText);

            return await ExecuteApiCallAsync(
                apiCall: () => _httpClient.GetAsync(path),
                successHandler: async response =>
                {
                    var requests = await response.Content.ReadFromJsonAsync<ListDataResponse<RequestTeamToIdea>>(Settings.BaseJsonOptions);
                    if (requests is null)
                    {
                        LogFail(GET_MARKET_TEAM_REQUESTS, response.StatusCode, "Error when parse team market requests");

                        return ServiceResponse<ListDataResponse<RequestTeamToIdea>>.Failure("Не удалось получить заявки в идеи", response.StatusCode);
                    }

                    return ServiceResponse<ListDataResponse<RequestTeamToIdea>>.Success(requests);
                },
                operationName: GET_MARKET_TEAM_REQUESTS
            );
        }

        public async Task<ServiceResponse<IEnumerable<RequestTeamToIdea>>> GetTeamRequestsForCurretnIdeaMarketAndTeamsAsync(
            Guid ideaMarketId,
            IEnumerable<Guid> currentTeamIds
        )
        {
            string path = $"{_teamPath}/market/requests";
            var content = SerializeData(new { IdeaMarketId = ideaMarketId, Teams = currentTeamIds });

            return await ExecuteApiCallAsync(
                apiCall: () => _httpClient.PostAsync(path, content),
                successHandler: async response =>
                {
                    var requests = await response.Content.ReadFromJsonAsync<IEnumerable<RequestTeamToIdea>>(Settings.BaseJsonOptions);
                    if (requests is null)
                    {
                        LogFail(GET_MARKET_TEAM_REQUESTS_IN_IDEA, response.StatusCode, "Error when parse team market requests");

                        return ServiceResponse<IEnumerable<RequestTeamToIdea>>.Failure("Не удалось получить заявки команд в идею", response.StatusCode);
                    }

                    return ServiceResponse<IEnumerable<RequestTeamToIdea>>.Success(requests);
                },
                operationName: GET_MARKET_TEAM_REQUESTS_IN_IDEA
            );
        }

        public async Task<ServiceResponse<RequestTeamToIdea>> SendRequestTeamToIdeaAsync(Guid teamId, Guid ideaMarketId, string? letter = null)
        {
            string path = $"{_teamPath}/market/request";
            var content = SerializeData(new { TeamId = teamId, IdeaMarketId = ideaMarketId, Letter = letter });

            return await ExecuteApiCallAsync(
                apiCall: () => _httpClient.PostAsync(path, content),
                successHandler: async response =>
                {
                    var requests = await response.Content.ReadFromJsonAsync<RequestTeamToIdea>(Settings.BaseJsonOptions);
                    if (requests is null)
                    {
                        LogFail(SEND_MARKET_TEAM_REQUEST, response.StatusCode, "Error when create request in idea");

                        return ServiceResponse<RequestTeamToIdea>.Failure("Не удалось отправить заявку в идею", response.StatusCode);
                    }

                    return ServiceResponse<RequestTeamToIdea>.Success(requests);
                },
                operationName: SEND_MARKET_TEAM_REQUEST
            );
        }

        public async Task<ServiceResponse<string>> UpdateRequestTeamToIdeaStatusAsync(Guid requestId, TeamRequestStatus newStatus)
        {
            string path = $"{_teamPath}/market/request/status/{requestId}/{newStatus}";

            return await ExecuteApiCallAsync(
                apiCall: () => _httpClient.PutAsync(path, null),
                successHandler: async response =>
                {
                    var message = await response.Content.ReadFromJsonAsync<MessageResponse>(Settings.BaseJsonOptions);
                    if (message is null)
                    {
                        LogFail(UPDATE_MARKET_TEAM_REQUEST_STATUS, response.StatusCode, "Error when update team request to idea status");

                        return ServiceResponse<string>.Failure("Не удалось изменить статус заявки в идею", response.StatusCode);
                    }

                    return ServiceResponse<string>.Success(message.Message);
                },
                operationName: UPDATE_MARKET_TEAM_REQUEST_STATUS
            );
        }
    }
}
