using HITSBlazor.Models.Common.Responses;
using HITSBlazor.Models.Users.Entities;
using HITSBlazor.Models.Users.Enums;
using HITSBlazor.Models.Users.Requests;
using HITSBlazor.Utils;
using System.Net;
using System.Net.Http.Json;

namespace HITSBlazor.Services.Users
{
    public class UserApi(
        IHttpClientFactory httpClientFactory,
        ILogger<UserApi> logger
    ) : BaseApiService(httpClientFactory.CreateClient(Settings.HttpClientName), logger)
    {
        private readonly string _userPath = "/api/users";

        private const string GET_USER_OPERATION = "GetUser";
        private const string GET_USERS_OPERATION = "GetUsers";
        private const string UPDATE_USER_OPERATION = "UpdateUser";
        private const string DELETE_USER_OPERATION = "DeleteUser";

        public async Task<ServiceResponse<User>> GetUserAsync(Guid? id = null)
        {
            string path = _userPath;
            if (id is not null && id.HasValue)
                path += $"/{id}";

            return await ExecuteApiCallAsync(
                apiCall: () => _httpClient.GetAsync(path),
                successHandler: async response =>
                {
                    User? user = await response.Content.ReadFromJsonAsync<User>(Settings.UserJsonOptions);
                    if (user is null)
                    {
                        LogFail(GET_USER_OPERATION, response.StatusCode, "Error when parse User model");

                        return ServiceResponse<User>.Failure("Не удалось получить пользователя", response.StatusCode);
                    }

                    if (id is not null && id.HasValue)
                        _logger.LogInformation("Get current user successful for user");
                    else
                        _logger.LogInformation("Get user successful for user");

                    return ServiceResponse<User>.Success(user);
                },
                operationName: GET_USER_OPERATION
            );
        }

        public async Task<ServiceResponse<ListDataResponse<User>>> GetUsersAsync(
            int page,
            int pageSize = 20,
            string? searchText = null,
            string? orderBy = null,
            bool? byDescending = null,
            bool? inTeam = null,
            Guid? ignoredTeam = null,
            IEnumerable<RoleType>? selectedRoles = null,
            IEnumerable<Guid>? ignoredIds = null
        )
        {
            string path = $"{_userPath}/all{AddQuery("page", page, false)}{AddQuery("page_size", pageSize)}";

            if (!string.IsNullOrWhiteSpace(searchText))
                path += AddQuery("search_text", searchText);

            if (!string.IsNullOrWhiteSpace(orderBy))
                path += AddQuery("order_by", orderBy);

            if (byDescending.HasValue)
                path += AddQuery("by_descending", byDescending.Value);

            if (inTeam.HasValue)
                path += AddQuery("in_team", inTeam.Value);

            if (ignoredTeam.HasValue)
                path += AddQuery("ignored_team", ignoredTeam.Value);

            if (selectedRoles is not null && selectedRoles.Any())
                path += AddQuery("selected_roles", selectedRoles);

            if (ignoredIds is not null && ignoredIds.Any())
                path += AddQuery("ignored_id", ignoredIds);

            return await ExecuteApiCallAsync(
                apiCall: () => _httpClient.GetAsync(path),
                successHandler: async response =>
                {
                    var users = await response.Content.ReadFromJsonAsync<ListDataResponse<User>>(Settings.UserJsonOptions);
                    if (users is null)
                    {
                        LogFail(GET_USERS_OPERATION, response.StatusCode, "Error when parse users");

                        return ServiceResponse<ListDataResponse<User>>.Failure("Не удалось получить пользователей", response.StatusCode);
                    }

                    return ServiceResponse<ListDataResponse<User>>.Success(users);
                },
                operationName: GET_USERS_OPERATION
            );
        }

        public async Task<ServiceResponse<string>> UpdateUserAsync(UpdateUserRequest request)
        {
            var content = SerializeData(request);

            return await ExecuteApiCallAsync(
                apiCall: () => _httpClient.PutAsync(_userPath, content),
                successHandler: async response =>
                {
                    var message = await response.Content.ReadFromJsonAsync<MessageResponse>(Settings.BaseJsonOptions);                    
                    if (message is null)
                    {
                        LogFail(UPDATE_USER_OPERATION, response.StatusCode, "Error when update user");

                        return ServiceResponse<string>.Failure("Не удалось обновить пользователя", response.StatusCode);
                    }

                    return ServiceResponse<string>.Success(message.Message);
                },
                operationName: UPDATE_USER_OPERATION
            );
        }

        public async Task<ServiceResponse<string>> DeleteUserAsync(Guid userId)
        {
            string path = $"{_userPath}/{userId}";

            return await ExecuteApiCallAsync(
                apiCall: () => _httpClient.DeleteAsync(path),
                successHandler: async response =>
                {
                    var message = await response.Content.ReadFromJsonAsync<MessageResponse>(Settings.BaseJsonOptions);
                    if (message is null)
                    {
                        LogFail(DELETE_USER_OPERATION, response.StatusCode, "Error when delete user");

                        return ServiceResponse<string>.Failure("Не удалось удалить пользователя", response.StatusCode);
                    }

                    return ServiceResponse<string>.Success(message.Message);
                },
                operationName: DELETE_USER_OPERATION
            );
        }

        protected override string GetErrorMessage(HttpStatusCode statusCode, string operationName) => operationName switch
        {
            GET_USER_OPERATION => statusCode switch
            {
                HttpStatusCode.Unauthorized => "Не удалось получить пользователя",
                HttpStatusCode.NotFound => "Не удалось получить пользователя",
                _ => base.GetErrorMessage(statusCode, operationName)
            },
            GET_USERS_OPERATION => statusCode switch
            {
                HttpStatusCode.Unauthorized => "Не удалось получить пользователей",
                _ => base.GetErrorMessage(statusCode, operationName)
            },
            UPDATE_USER_OPERATION => statusCode switch
            {
                HttpStatusCode.Forbidden => "Отказано в доступе",
                HttpStatusCode.NotFound => "Пользователь не найден",
                HttpStatusCode.UnprocessableEntity => "Не удалось обновить пользователя, проверьте корректность данных",
                _ => base.GetErrorMessage(statusCode, operationName)
            },
            DELETE_USER_OPERATION => statusCode switch
            {
                HttpStatusCode.Forbidden => "Отказано в доступе",
                HttpStatusCode.NotFound => "Пользователь не найден",
                _ => base.GetErrorMessage(statusCode, operationName)
            },
            _ => base.GetErrorMessage(statusCode, operationName)
        };
    }
}
