using HITSBlazor.Models.Common.Entities;
using HITSBlazor.Models.Common.Responses;
using HITSBlazor.Models.Users.Entities;
using HITSBlazor.Models.Users.Enums;
using HITSBlazor.Utils;
using System.Net.Http.Json;

namespace HITSBlazor.Services.UsersGroups
{
    public class UsersGroupApi(
        IHttpClientFactory httpClientFactory,
        ILogger<UsersGroupApi> logger
    ) : BaseApiService(httpClientFactory.CreateClient(Settings.HttpClientName), logger)
    {
        private readonly string _usersGroupPath = "/api/group";

        private const string GET_GROUPS_OPERATION = "GetGroups";
        private const string GET_GROUP_OPERATION = "GetGroup";
        private const string GET_GROUP_MEMBERS_OPERATION = "GetGroupMembers";
        private const string CREATE_GROUP_OPERATION = "CreateGroup";
        private const string UPDATE_GROUP_OPERATION = "UpdateGroup";
        private const string DELETE_GROUP_OPERATION = "DeleteGroup";

        public async Task<ServiceResponse<ListDataResponse<UsersGroup>>> GetGroupsAsync(
            int page,
            int pageSize = 20,
            string? searchText = null,
            IEnumerable<RoleType>? selectedRoles = null
        )
        {
            string path = $"{_usersGroupPath}{AddQuery("page", page, false)}{AddQuery("page_size", pageSize)}";

            if (!string.IsNullOrWhiteSpace(searchText))
                path += AddQuery("search_text", searchText);

            if (selectedRoles is not null && selectedRoles.Any())
                path += AddQuery("selected_role", selectedRoles);

            return await ExecuteApiCallAsync(
                apiCall: () => _httpClient.GetAsync(path),
                successHandler: async response =>
                {
                    var groups = await response.Content.ReadFromJsonAsync<ListDataResponse<UsersGroup>>(Settings.BaseJsonOptions);
                    if (groups is null)
                    {
                        LogFail(GET_GROUPS_OPERATION, response.StatusCode, "Error when parse groups");

                        return ServiceResponse<ListDataResponse<UsersGroup>>.Failure("Не удалось получить группы", response.StatusCode);
                    }

                    return ServiceResponse<ListDataResponse<UsersGroup>>.Success(groups);
                },
                operationName: GET_GROUPS_OPERATION
            );
        }

        public async Task<ServiceResponse<UsersGroup?>> GetGroupAsync(Guid groupId)
        {
            string path = $"{_usersGroupPath}/{groupId}";

            return await ExecuteApiCallAsync(
                apiCall: () => _httpClient.GetAsync(path),
                successHandler: async response =>
                {
                    var group = await response.Content.ReadFromJsonAsync<UsersGroup>(Settings.BaseJsonOptions);
                    if (group is null)
                    {
                        LogFail(GET_GROUP_OPERATION, response.StatusCode, "Error when parse group");

                        return ServiceResponse<UsersGroup?>.Failure("Не удалось получить группу", response.StatusCode);
                    }

                    return ServiceResponse<UsersGroup?>.Success(group);
                },
                operationName: GET_GROUP_OPERATION
            );
        }

        public async Task<ServiceResponse<ListDataResponse<User>>> GetGroupMembersAsync(
            Guid groupId,
            int page,
            int pageSize = 20,
            string? searchText = null
        )
        {
            string path = $"{_usersGroupPath}/{groupId}/members{AddQuery("page", page, false)}{AddQuery("page_size", pageSize)}";

            if (!string.IsNullOrWhiteSpace(searchText))
                path += AddQuery("search_text", searchText);

            return await ExecuteApiCallAsync(
                apiCall: () => _httpClient.GetAsync(path),
                successHandler: async response =>
                {
                    var members = await response.Content.ReadFromJsonAsync<ListDataResponse<User>>(Settings.BaseJsonOptions);
                    if (members is null)
                    {
                        LogFail(GET_GROUP_MEMBERS_OPERATION, response.StatusCode, "Error when parse group members");

                        return ServiceResponse<ListDataResponse<User>>.Failure("Не удалось получить участников группы", response.StatusCode);
                    }

                    return ServiceResponse<ListDataResponse<User>>.Success(members);
                },
                operationName: GET_GROUP_MEMBERS_OPERATION
            );
        }

        public async Task<ServiceResponse<UsersGroup?>> CreateGroupAsync(
            string name, 
            IEnumerable<RoleType> roles, 
            IEnumerable<User> members
        )
        {
            var content = SerializeData(new { Name = name, Roles = roles, Members = members });

            return await ExecuteApiCallAsync(
                apiCall: () => _httpClient.PostAsync(_usersGroupPath, content),
                successHandler: async response =>
                {
                    var group = await response.Content.ReadFromJsonAsync<UsersGroup>(Settings.BaseJsonOptions);
                    if (group is null)
                    {
                        LogFail(CREATE_GROUP_OPERATION, response.StatusCode, "Error when create group");

                        return ServiceResponse<UsersGroup?>.Failure("Не удалось создать группу", response.StatusCode);
                    }

                    return ServiceResponse<UsersGroup?>.Success(group);
                },
                operationName: CREATE_GROUP_OPERATION
            );
        }

        public async Task<ServiceResponse<UsersGroup?>> UpdateGroupAsync(
            Guid usersGroupId,
            string? name = null,
            IEnumerable<RoleType>? roles = null,
            IEnumerable<Guid>? newMembersIds = null,
            IEnumerable<Guid>? removeMembersIds = null
        )
        {
            var content = SerializeData(new { Id = usersGroupId, Name = name, Roles = roles, NewMembersIds = newMembersIds, RemoveMembersIds = removeMembersIds });

            return await ExecuteApiCallAsync(
                apiCall: () => _httpClient.PutAsync(_usersGroupPath, content),
                successHandler: async response =>
                {
                    var group = await response.Content.ReadFromJsonAsync<UsersGroup>(Settings.BaseJsonOptions);
                    if (group is null)
                    {
                        LogFail(UPDATE_GROUP_OPERATION, response.StatusCode, "Error when update group");

                        return ServiceResponse<UsersGroup?>.Failure("Не удалось обновить группу", response.StatusCode);
                    }

                    return ServiceResponse<UsersGroup?>.Success(group);
                },
                operationName: UPDATE_GROUP_OPERATION
            );
        }

        public async Task<ServiceResponse<string>> DeleteGroupAsync(Guid groupId)
        {
            string path = $"{_usersGroupPath}/{groupId}";

            return await ExecuteApiCallAsync(
                apiCall: () => _httpClient.DeleteAsync(path),
                successHandler: async response =>
                {
                    var message = await response.Content.ReadFromJsonAsync<MessageResponse>(Settings.BaseJsonOptions);
                    if (message is null)
                    {
                        LogFail(DELETE_GROUP_OPERATION, response.StatusCode, "Error when delete group");

                        return ServiceResponse<string>.Failure("Не удалось удалить группу", response.StatusCode);
                    }

                    return ServiceResponse<string>.Success(message.Message);
                },
                operationName: DELETE_GROUP_OPERATION
            );
        }
    }
}
