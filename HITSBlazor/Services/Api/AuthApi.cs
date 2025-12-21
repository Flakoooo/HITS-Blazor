using HITSBlazor.Models.Auth.Requests;
using HITSBlazor.Utils;
using System.Net;
using System.Net.Http.Json;

namespace HITSBlazor.Services.Api
{
    public class AuthApi(
        IHttpClientFactory httpClientFactory,
        ICookieService cookieService,
        ILogger<AuthApi> logger)
    {
        private readonly HttpClient _httpClient = httpClientFactory.CreateClient("ApiClient");
        private readonly ICookieService _cookieService = cookieService;
        private readonly ILogger<AuthApi> _logger = logger;
        private readonly string _authPath = "/auth";

        public async Task<ApiResponse<string>> LoginAsync(LoginRequest request)
        {
            try
            {
                _logger.LogInformation("Sending login request to {Path}/login", _authPath);

                await _cookieService.DeleteCookie("refresh_token");

                var response = await _httpClient.PostAsJsonAsync($"{_authPath}/login", request);
                if (response.IsSuccessStatusCode)
                {
                    var refreshToken = await _cookieService.GetCookieAsync("refresh_token");
                    if (string.IsNullOrEmpty(refreshToken))
                    {
                        await Task.Delay(100);
                        refreshToken = await _cookieService.GetCookieAsync("refresh_token");
                    }

                    if (string.IsNullOrEmpty(refreshToken))
                    {
                        _logger.LogWarning("Refresh token cookie not found after login");
                        return ApiResponse<string>.Failure(
                            "Токен обновления не найден",
                            HttpStatusCode.Unauthorized
                        );
                    }

                    string? accessToken = null;
                    if (response.Headers.TryGetValues("Authorization", out var authHeaders))
                    {
                        var authHeader = authHeaders.FirstOrDefault();
                        if (!string.IsNullOrEmpty(authHeader) && authHeader.StartsWith("Bearer "))
                            accessToken = authHeader["Bearer ".Length..];
                    }

                    if (string.IsNullOrEmpty(accessToken))
                    {
                        _logger.LogWarning("Access token not found in response");
                        return ApiResponse<string>.Failure(
                            "Токен доступа не найден",
                            HttpStatusCode.Unauthorized
                        );
                    }

                    _logger.LogInformation("Login successful for user {Email}", request.Email);
                    return ApiResponse<string>.Success(accessToken);
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

                return ApiResponse<string>.Failure(
                    errorMessage,
                    response.StatusCode
                );
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "Network error during login");
                return ApiResponse<string>.Failure(
                    $"Ошибка сети: {ex.Message}",
                    HttpStatusCode.ServiceUnavailable
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error during login");
                return ApiResponse<string>.Failure(
                    $"Произошла ошибка: {ex.Message}",
                    HttpStatusCode.InternalServerError
                );
            }
        }

        public async Task<ApiResponse<string>> RefreshTokenAsync()
        {
            try
            {
                _logger.LogInformation("Refreshing token...");

                var response = await _httpClient.PostAsync($"{_authPath}/refresh", null);
                if (response.IsSuccessStatusCode)
                {
                    _logger.LogInformation("Token refresh successful");

                    string? newAccessToken = null;
                    if (response.Headers.TryGetValues("Authorization", out var authHeaders))
                    {
                        var authHeader = authHeaders.FirstOrDefault();
                        if (!string.IsNullOrEmpty(authHeader) && authHeader.StartsWith("Bearer "))
                            newAccessToken = authHeader["Bearer ".Length..];
                    }

                    if (string.IsNullOrEmpty(newAccessToken))
                        return ApiResponse<string>.Failure("Новый токен доступа не найден в ответе");

                    return ApiResponse<string>.Success(newAccessToken);
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

                await _cookieService.DeleteCookie("refresh_token");

                return ApiResponse<string>.Failure(
                    "Не удалось обновить токен. Требуется повторный вход",
                    response.StatusCode
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during token refresh");
                return ApiResponse<string>.Failure($"Ошибка: {ex.Message}");
            }
        }
    }
}
