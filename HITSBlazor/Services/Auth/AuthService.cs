using HITSBlazor.Models.Users.Entities;
using HITSBlazor.Models.Users.Enums;
using HITSBlazor.Pages.Login;
using HITSBlazor.Pages.NewPassword;
using HITSBlazor.Pages.Register;
using HITSBlazor.Utils;
using Microsoft.AspNetCore.Components;

namespace HITSBlazor.Services.Auth
{
    public class AuthService(
        ILogger<AuthService> logger,
        AuthApi authApi, 
        NavigationManager navigationManager
    ) : IAuthService
    {
        private readonly ILogger<AuthService> _logger = logger;
        private readonly AuthApi _authApi = authApi;
        private readonly NavigationManager _navigationManager = navigationManager;

        public event Action? OnAuthStateChanged;
        public bool IsAuthenticated { get; private set; } = false;
        public User CurrentUser { get; private set; } = new();

        public async Task InitializeAsync()
        {
            if (AppEnvironment.IsLogEnabled)
                _logger.LogInformation("Initializing auth service...");

            IsAuthenticated = (await _authApi.RefreshTokenAsync()).IsSuccess;

            OnAuthStateChanged?.Invoke();
            return;
        }

        public async Task<ServiceResponse<bool>> LoginAsync(LoginModel request)
        {
            if (AppEnvironment.IsLogEnabled && _logger.IsEnabled(LogLevel.Information))
                _logger.LogInformation("Attempting login for email: {Email}", request.Email);

            var result = await _authApi.LoginAsync(request);
            if (result.IsSuccess)
            {
                IsAuthenticated = true;
                OnAuthStateChanged?.Invoke();
            }
            else if (_logger.IsEnabled(LogLevel.Warning))
                _logger.LogWarning("Login failed: {Error}", result.Message);

            return result;
        }

        public async Task<ServiceResponse<bool>> LogoutAsync()
        {
            if (AppEnvironment.IsLogEnabled)
                _logger.LogInformation("Logging out user...");

            var result = await _authApi.LogoutAsync();
            if (result.IsSuccess)
            {
                IsAuthenticated = false;
                OnAuthStateChanged?.Invoke();
                _navigationManager.NavigateTo("/login");
            }
            else if (_logger.IsEnabled(LogLevel.Warning))
                _logger.LogWarning("Login failed: {Error}", result.Message);

            return result;
        }

        public async Task<ServiceResponse<Guid>> RequestPasswordRecoveryAsync(string email)
        {
            if (AppEnvironment.IsLogEnabled && _logger.IsEnabled(LogLevel.Information))
                _logger.LogInformation("Attempting password recovery for email: {Email}", email);

            var result = await _authApi.PasswordVerificationAsync(email);
            if (!result.IsSuccess && _logger.IsEnabled(LogLevel.Warning))
                _logger.LogWarning("REcovery password failed: {Error}", result.Message);

            return result;
        }

        public async Task<ServiceResponse<bool>> ResetPasswordAsync(NewPasswordModel newPasswordModel)
        {
            if (AppEnvironment.IsLogEnabled)
                _logger.LogInformation("Attempting update password");

            var result = await _authApi.PasswordNewAsync(newPasswordModel);
            if (!result.IsSuccess && _logger.IsEnabled(LogLevel.Warning))
                _logger.LogWarning("Reset password failed: {Error}", result.Message);

            return result;
        }

        public async Task<ServiceResponse<bool>> RegistrationAsync(RegisterModel request, Guid invitationId)
        {
            if (AppEnvironment.IsLogEnabled)
                _logger.LogInformation("Attempting user registration");

            var result = await _authApi.RegistrationUserAsync(request, invitationId);
            if (result.IsSuccess)
            {
                IsAuthenticated = true;
                OnAuthStateChanged?.Invoke();
            }
            else if (_logger.IsEnabled(LogLevel.Warning))
                _logger.LogWarning("Registration failed: {Error}", result.Message);

            return result;
        }

        public bool SetUserRoleAsync(RoleType roleType)
        {
            if (CurrentUser == null || !CurrentUser.Roles.Contains(roleType))
                return false;

            CurrentUser.Role = roleType;

            OnAuthStateChanged?.Invoke();

            return true;
        }
    }
}
