using HITSBlazor.Models.Auth.Requests;
using HITSBlazor.Models.Users.Entities;
using HITSBlazor.Utils;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;

namespace HITSBlazor.Services.Api
{
    public class AuthApi
    {
        private readonly HttpClient _httpClient;
        private readonly ICookieService _cookieService;
        private readonly ILogger<AuthApi> _logger;
        private readonly string _authPath;

        public AuthApi(
            HttpClient httpClient,
            AppSettings settings,
            ICookieService cookieService,
            ILogger<AuthApi> logger)
        {
            _httpClient = httpClient;
            _cookieService = cookieService;
            _logger = logger;
            _authPath = $"{settings.ApiBaseUrl}/auth";

            //_httpClient.DefaultRequestHeaders.Add("X-Requested-With", "XMLHttpRequest");
        }

        public async Task<ApiResponse<User>> LoginAsync(LoginRequest request)
        {
            try
            {
                _logger.LogInformation("Sending login request to {Path}/login", _authPath);

                var response = await _httpClient.PostAsJsonAsync($"{_authPath}/login", request);

                if (response.IsSuccessStatusCode)
                {
                    bool hasCookies = response.Headers.TryGetValues("Set-Cookie", out var _);
                    if (!hasCookies)
                    {
                        _logger.LogWarning("No Set-Cookie header in response");
                        return ApiResponse<User>.Failure(
                            "Не удалось установить токен аутентификации",
                            HttpStatusCode.Unauthorized
                        );
                    }

                    var token = await _cookieService.GetCookieAsync("access_token");
                    if (string.IsNullOrEmpty(token))
                    {
                        await Task.Delay(100);
                        token = await _cookieService.GetCookieAsync("access_token");
                    }

                    if (string.IsNullOrEmpty(token))
                    {
                        _logger.LogWarning("Access token cookie not found after login");
                        return ApiResponse<User>.Failure(
                            "Токен аутентификации не найден",
                            HttpStatusCode.Unauthorized
                        );
                    }

                    var user = JwtHelper.DecodeJwtPayload(token);
                    if (user == null)
                    {
                        _logger.LogWarning("Failed to decode JWT token");
                        return ApiResponse<User>.Failure(
                            "Неверный формат токена",
                            HttpStatusCode.Unauthorized
                        );
                    }

                    _logger.LogInformation("Login successful for user {Email}", user.Email);
                    return ApiResponse<User>.Success(user);
                }

                var content = await response.Content.ReadAsStringAsync();
                string errorMessage;

                try
                {
                    var json = JsonDocument.Parse(content);
                    if (json.RootElement.TryGetProperty("message", out var message))
                    {
                        errorMessage = message.GetString() ?? "Ошибка при входе";
                    }
                    else if (json.RootElement.TryGetProperty("error", out var error))
                    {
                        errorMessage = error.GetString() ?? "Ошибка при входе";
                    }
                    else
                    {
                        errorMessage = content.Length > 100 ? string.Concat(content.AsSpan(0, 100), "...") : content;
                    }
                }
                catch
                {
                    errorMessage = response.StatusCode switch
                    {
                        HttpStatusCode.Unauthorized => "Неверный email или пароль",
                        HttpStatusCode.BadRequest => "Неверный формат запроса",
                        HttpStatusCode.NotFound => "Сервис авторизации недоступен",
                        _ => $"Ошибка сервера: {response.StatusCode}"
                    };
                }

                _logger.LogWarning("Login failed with status {StatusCode}: {Error}",
                    response.StatusCode, errorMessage);

                return ApiResponse<User>.Failure(
                    errorMessage,
                    response.StatusCode
                );
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "Network error during login");
                return ApiResponse<User>.Failure(
                    $"Ошибка сети: {ex.Message}",
                    HttpStatusCode.ServiceUnavailable
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error during login");
                return ApiResponse<User>.Failure(
                    $"Произошла ошибка: {ex.Message}",
                    HttpStatusCode.InternalServerError
                );
            }
        }

        public async Task<ApiResponse<User>> RefreshTokenAsync()
        {
            try
            {
                _logger.LogInformation("Refreshing token...");

                var response = await _httpClient.PostAsync($"{_authPath}/refresh", null);

                if (response.IsSuccessStatusCode)
                {
                    _logger.LogInformation("Token refresh successful");

                    await Task.Delay(100);

                    var token = await _cookieService.GetCookieAsync("access_token");
                    if (!string.IsNullOrEmpty(token))
                    {
                        var user = JwtHelper.DecodeJwtPayload(token);
                        return user != null
                            ? ApiResponse<User>.Success(user)
                            : ApiResponse<User>.Failure("Не удалось декодировать токен");
                    }

                    return ApiResponse<User>.Failure("Токен не найден после обновления");
                }

                var error = await response.Content.ReadAsStringAsync();
                _logger.LogWarning("Token refresh failed: {Error}", error);

                return ApiResponse<User>.Failure(
                    "Не удалось обновить токен",
                    response.StatusCode
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during token refresh");
                return ApiResponse<User>.Failure($"Ошибка: {ex.Message}");
            }
        }
    }
}
