using HITSBlazor.Models.Auth.Requests;
using HITSBlazor.Utils;
using System.Net;
using System.Net.Http.Json;

namespace HITSBlazor.Services.Api
{
    public class AuthApi(
        IHttpClientFactory httpClientFactory,
        ILogger<AuthApi> logger)
    {
        private readonly HttpClient _httpClient = httpClientFactory.CreateClient("HITSClient");
        private readonly ILogger<AuthApi> _logger = logger;
        private readonly string _authPath = "/api/auth";

        public async Task<ApiResponse<bool>> LoginAsync(LoginRequest request)
        {
            try
            {
                _logger.LogInformation("Sending login request to {Path}/login", _authPath);

                var response = await _httpClient.PostAsJsonAsync($"{_authPath}/login", request);
                if (response.IsSuccessStatusCode)
                {
                    _logger.LogInformation("Login successful for user {Email}", request.Email);
                    return ApiResponse<bool>.Success(true);
                }

                string errorMessage = response.StatusCode switch
                {
                    HttpStatusCode.Unauthorized => "Неверный email или пароль",
                    HttpStatusCode.UnprocessableContent => "Некорректный email или пароль",
                    HttpStatusCode.BadRequest => "Неверный формат запроса",
                    HttpStatusCode.NotFound => "Сервис авторизации недоступен",
                    HttpStatusCode.InternalServerError => "Произошла ошибка сервера, попробуйте позже",
                    _ => $"Ошибка сервера: {response.StatusCode}"
                };

                _logger.LogWarning(
                    "Login failed with status {StatusCode}: {Error}",
                    response.StatusCode, errorMessage
                );

                return ApiResponse<bool>.Failure(
                    errorMessage,
                    response.StatusCode
                );
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "Network error during login");
                return ApiResponse<bool>.Failure(
                    $"Ошибка сети: {ex.Message}",
                    HttpStatusCode.ServiceUnavailable
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error during login");
                return ApiResponse<bool>.Failure(
                    $"Произошла ошибка: {ex.Message}",
                    HttpStatusCode.InternalServerError
                );
            }
        }

        public async Task<ApiResponse<bool>> RefreshTokenAsync()
        {
            try
            {
                _logger.LogInformation("Refreshing token...");

                var response = await _httpClient.PostAsync($"{_authPath}/refresh", null);
                if (response.IsSuccessStatusCode)
                {
                    _logger.LogInformation("Token refresh successful");
                    return ApiResponse<bool>.Success(true);
                }

                string errorMessage = response.StatusCode switch
                {
                    HttpStatusCode.Unauthorized => "Не удалось обновить токен. Требуется повторный вход",
                    _ => $"Ошибка сервера: {response.StatusCode}"
                };

                _logger.LogWarning(
                    "Token refresh failed with status {StatusCode}: {Error}",
                    response.StatusCode, errorMessage
                );

                return ApiResponse<bool>.Failure(
                    "Не удалось обновить токен. Требуется повторный вход",
                    response.StatusCode
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during token refresh");
                return ApiResponse<bool>.Failure($"Ошибка: {ex.Message}");
            }
        }
    }
}
