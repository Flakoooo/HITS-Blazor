using HITSBlazor.Models.Quests.Entities;
using HITSBlazor.Models.Users.Entities;
using HITSBlazor.Utils;
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
                        if (_logger.IsEnabled(LogLevel.Warning))
                            _logger.LogWarning(
                                "{Operation} failed: {StatusCode} - {ErrorMessage}",
                                GET_USER_OPERATION, response.StatusCode, "Error when parse User model"
                            );

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
    }
}
