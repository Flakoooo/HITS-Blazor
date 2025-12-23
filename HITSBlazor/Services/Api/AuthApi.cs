using HITSBlazor.Models.Auth.Requests;
using HITSBlazor.Utils;
using System.Net;
using System.Net.Http.Json;

namespace HITSBlazor.Services.Api
{
    public class AuthApi(
        IHttpClientFactory httpClientFactory, 
        ILogger<AuthApi> logger
    ) : BaseApiService(httpClientFactory.CreateClient("HITSClient"), logger)
    {
        private readonly string _authPath = "/api/auth";

        public Task<ApiResponse<bool>> LoginAsync(LoginRequest request)
        {
            return ExecuteApiCallAsync(
                apiCall: () => _httpClient.PostAsJsonAsync($"{_authPath}/login", request),
                successHandler: async response =>
                {
                    if (AppEnvironment.IsLogEnabled && _logger.IsEnabled(LogLevel.Information))
                        _logger.LogInformation("Login successful for user {Email}", request.Email);

                    return ApiResponse<bool>.Success(true);
                },
                operationName: "Login"
            );
        }

        public Task<ApiResponse<bool>> RefreshTokenAsync()
        {
            return ExecuteApiCallAsync(
                apiCall: () => _httpClient.PostAsync($"{_authPath}/refresh", null),
                successHandler: async response =>
                {
                    if (AppEnvironment.IsLogEnabled)
                        _logger.LogInformation("Token refresh successful");

                    return ApiResponse<bool>.Success(true);
                },
                operationName: "RefreshToken"
            );
        }

        protected override string GetErrorMessage(HttpStatusCode statusCode, string operationName)
        {
            return operationName switch
            {
                "Login" => statusCode switch
                {
                    HttpStatusCode.Unauthorized => "Неверный email или пароль",
                    HttpStatusCode.UnprocessableEntity => "Некорректный email или пароль",
                    HttpStatusCode.BadRequest => "Неверный формат запроса",
                    HttpStatusCode.NotFound => "Сервис авторизации недоступен",
                    HttpStatusCode.InternalServerError => "Произошла ошибка сервера, попробуйте позже",
                    _ => base.GetErrorMessage(statusCode, operationName)
                },
                "RefreshToken" => statusCode switch
                {
                    HttpStatusCode.Unauthorized => "Не удалось обновить токен. Требуется повторный вход",
                    _ => base.GetErrorMessage(statusCode, operationName)
                },
                _ => base.GetErrorMessage(statusCode, operationName)
            };
        }
    }
}
