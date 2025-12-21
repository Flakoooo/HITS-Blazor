using Blazored.LocalStorage;
using HITSBlazor.Models.Auth.Requests;
using HITSBlazor.Models.Auth.Response;
using HITSBlazor.Models.Users.Entities;
using HITSBlazor.Services.Api;
using HITSBlazor.Services.Service.Interfaces;
using HITSBlazor.Utils;
using Microsoft.AspNetCore.Components;

namespace HITSBlazor.Services.Service.Class
{
    public class AuthService(
        ILogger<AuthService> logger,
        ICookieService cookieService,
        AuthApi authApi, 
        NavigationManager navigationManager,
        ILocalStorageService localStorage
    ) : IAuthService
    {
        private readonly ILogger<AuthService> _logger = logger;
        private readonly AuthApi _authApi = authApi;
        private readonly ICookieService _cookieService = cookieService;
        private readonly NavigationManager _navigationManager = navigationManager;
        private readonly ILocalStorageService _localStorage = localStorage;

        public event Action? OnAuthStateChanged;
        public User? CurrentUser { get; private set; }
        public bool IsAuthenticated => CurrentUser != null;
        private string? _accessToken;
        private readonly SemaphoreSlim _refreshLock = new(1, 1);

        public async Task InitializeAsync()
        {
            try
            {
                _logger.LogInformation("Initializing auth service...");

                _accessToken = await _localStorage.GetItemAsStringAsync("access_token");
                if (string.IsNullOrEmpty(_accessToken))
                {
                    _logger.LogInformation("No access token found");
                    await SetCurrentUserAsync(null);
                    return;
                }

                if (JwtHelper.IsTokenExpired(_accessToken))
                {
                    _logger.LogInformation("Access token expired on initialization");

                    var newToken = await GetAccessTokenAsync(forceRefresh: true);
                    if (string.IsNullOrEmpty(newToken)) await SetCurrentUserAsync(null);
                    return;
                }

                var user = JwtHelper.DecodeJwtPayload(_accessToken);
                await SetCurrentUserAsync(user);

                if (user != null)
                {
                    _logger.LogInformation("User initialized: {Email}", user.Email);
                }
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
                if (result.IsSuccess && !string.IsNullOrWhiteSpace(result.Response))
                {
                    _accessToken = result.Response;
                    await _localStorage.SetItemAsStringAsync("access_token", _accessToken);

                    CurrentUser = JwtHelper.DecodeJwtPayload(_accessToken);
                    if (CurrentUser != null)
                    {
                        _logger.LogInformation("Login successful for user: {Email}", CurrentUser.Email);
                        OnAuthStateChanged?.Invoke();
                        return LoginResponse.Success("Успешный вход", CurrentUser);
                    }
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
                _logger.LogInformation("Logging out user...");

                await _localStorage.RemoveItemAsync("access_token");
                await _cookieService.DeleteCookie("refresh_token");

                await SetCurrentUserAsync(null);

                _navigationManager.NavigateTo("/login", true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during logout");
            }
        }

        private async Task SetCurrentUserAsync(User? user)
        {
            CurrentUser = user;
            _accessToken = null;
            OnAuthStateChanged?.Invoke();

            if (user == null)
            {
                await _localStorage.RemoveItemAsync("access_token");
                await _cookieService.DeleteCookie("refresh_token");
            }
        }

        public async Task<string?> GetAccessTokenAsync(bool forceRefresh = false)
        {
            try
            {
                if (!forceRefresh && !string.IsNullOrEmpty(_accessToken) &&
                    !JwtHelper.IsTokenExpired(_accessToken)) return _accessToken;

                await _refreshLock.WaitAsync();

                try
                {
                    if (!forceRefresh && !string.IsNullOrEmpty(_accessToken) &&
                        !JwtHelper.IsTokenExpired(_accessToken)) return _accessToken;

                    _logger.LogInformation("Attempting to refresh access token...");

                    var refreshResult = await _authApi.RefreshTokenAsync();
                    if (refreshResult.IsSuccess && !string.IsNullOrEmpty(refreshResult.Response))
                    {
                        _accessToken = refreshResult.Response;
                        await _localStorage.SetItemAsStringAsync("access_token", _accessToken);

                        var user = JwtHelper.DecodeJwtPayload(_accessToken);
                        if (user != null)
                        {
                            CurrentUser = user;
                            OnAuthStateChanged?.Invoke();
                        }

                        return _accessToken;
                    }
                    else
                    {
                        _logger.LogWarning("Refresh token failed: {Error}", refreshResult.Message);

                        await LogoutAsync();
                        return null;
                    }
                }
                finally
                {
                    _refreshLock.Release();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting access token");
                return null;
            }
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
