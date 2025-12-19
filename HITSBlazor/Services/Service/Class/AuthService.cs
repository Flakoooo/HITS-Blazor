using HITSBlazor.Models.Auth.Requests;
using HITSBlazor.Models.Auth.Response;
using HITSBlazor.Models.Users.Entities;
using HITSBlazor.Models.Users.Enums;
using HITSBlazor.Services.Api;
using HITSBlazor.Services.Service.Interfaces;
using HITSBlazor.Utils;
using Microsoft.AspNetCore.Components;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.JSInterop;
using System.Text.Json;

namespace HITSBlazor.Services.Service.Class
{
    public class AuthService(
        ILogger<AuthService> logger,
        ICookieService cookieService,
        AuthApi authApi, 
        NavigationManager navigationManager
    ) : IAuthService
    {
        private readonly ILogger<AuthService> _logger = logger;
        private readonly AuthApi _authApi = authApi;
        private readonly ICookieService _cookieService = cookieService;
        private readonly NavigationManager _navigationManager = navigationManager;

        public event Action? OnAuthStateChanged;
        public User? CurrentUser { get; private set; }
        public bool IsAuthenticated => CurrentUser != null;


        public async Task InitializeAsync()
        {
            try
            {
                _logger.LogInformation("Initializing auth service...");

                // Проверяем наличие access токена
                var accessToken = await _cookieService.GetCookieAsync("access_token");
                if (string.IsNullOrEmpty(accessToken))
                {
                    _logger.LogInformation("No access token found");
                    await SetCurrentUserAsync(null);
                    return;
                }

                _logger.LogInformation("Access token found, checking expiration...");

                // Проверяем, не истек ли токен
                if (JwtHelper.IsTokenExpired(accessToken))
                {
                    _logger.LogInformation("Access token expired, attempting refresh...");

                    // Пытаемся обновить токен
                    var refreshResult = await _authApi.RefreshTokenAsync();

                    if (!refreshResult.IsSuccess)
                    {
                        _logger.LogWarning("Token refresh failed: {Error}", refreshResult.Message);
                        await SetCurrentUserAsync(null);
                        return;
                    }

                    _logger.LogInformation("Token refreshed successfully");

                    // Получаем новый токен из куки
                    accessToken = await _cookieService.GetCookieAsync("access_token");
                }

                if (!string.IsNullOrEmpty(accessToken))
                {
                    _logger.LogInformation("Decoding JWT token...");
                    CurrentUser = JwtHelper.DecodeJwtPayload(accessToken);

                    if (CurrentUser != null)
                    {
                        _logger.LogInformation("User initialized: {Email}", CurrentUser.Email);
                        OnAuthStateChanged?.Invoke();
                        return;
                    }
                    else
                    {
                        _logger.LogWarning("Failed to decode JWT token");
                    }
                }

                await SetCurrentUserAsync(null);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during auth initialization");
                await SetCurrentUserAsync(null);
            }
        }

        public async Task<LoginResponse> LoginAsync(LoginRequest request)
        {
            try
            {
                _logger.LogInformation("Attempting login for email: {Email}", request.Email);

                var result = await _authApi.LoginAsync(request);

                if (result.IsSuccess)
                {
                    var token = await _cookieService.GetCookieAsync("access_token");

                    if (!string.IsNullOrEmpty(token))
                    {
                        CurrentUser = JwtHelper.DecodeJwtPayload(token);

                        if (CurrentUser != null)
                        {
                            _logger.LogInformation("Login successful for user: {Email}", CurrentUser.Email);
                            OnAuthStateChanged?.Invoke();
                            return LoginResponse.Success("Успешный вход", CurrentUser);
                        }
                    }

                    _logger.LogWarning("Token not found in cookies after login");
                    return LoginResponse.Failure("Не удалось получить токен аутентификации");
                }

                _logger.LogWarning("Login failed: {Error}", result.Message);
                return LoginResponse.Failure(result.Message ?? "Ошибка входа");
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "Network error during login");
                return LoginResponse.Failure($"Ошибка сети: {ex.Message}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error during login");
                return LoginResponse.Failure($"Произошла ошибка: {ex.Message}");
            }
        }

        public async Task LogoutAsync()
        {
            try
            {
                _logger.LogInformation("Logging out user");

                await _cookieService.DeleteCookie("access_token");
                await _cookieService.DeleteCookie("refresh_token");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during logout");
            }
            finally
            {
                await SetCurrentUserAsync(null);
                _navigationManager.NavigateTo("/login", true);
            }
        }

        private async Task SetCurrentUserAsync(User? user)
        {
            CurrentUser = user;
            OnAuthStateChanged?.Invoke();

            if (user == null)
            {
                await _cookieService.DeleteCookie("access_token");
                await _cookieService.DeleteCookie("refresh_token");
            }
        }

        public async Task<string?> GetAccessTokenAsync()
        {
            return await _cookieService.GetCookieAsync("access_token");
        }

        public Task<RecoveryResponse> RequestPasswordRecoveryAsync(string email)
        {
            throw new NotImplementedException();
        }

        public Task<ResetPasswordResponse> ResetPasswordAsync(ResetPasswordRequest resetPassword)
        {
            throw new NotImplementedException();
        }

        public Task<RegisterResponse> RegisterAsync(RegisterRequest request, string? invitationCode = null)
        {
            throw new NotImplementedException();
        }
    }
}
