using HITSBlazor.Models.Users.Entities;
using HITSBlazor.Models.Users.Enums;
using HITSBlazor.Pages.Login;
using HITSBlazor.Pages.NewPassword;
using HITSBlazor.Pages.RecoveryPassword;
using HITSBlazor.Pages.Register;
using HITSBlazor.Services.Users;
using HITSBlazor.Utils;
using Microsoft.AspNetCore.Components;

namespace HITSBlazor.Services.Auth
{
    public class AuthService(
        ILogger<AuthService> logger,
        AuthApi authApi,
        UserApi userApi,
        NavigationManager navigationManager,
        GlobalNotificationService globalNotificationService
    ) : IAuthService
    {
        private readonly ILogger<AuthService> _logger = logger;
        private readonly AuthApi _authApi = authApi;
        private readonly UserApi _userApi = userApi;
        private readonly NavigationManager _navigationManager = navigationManager;
        private readonly GlobalNotificationService _globalNotificationService = globalNotificationService;

        private readonly CommonAuthLogic _commonAuthLogic = new(globalNotificationService);

        public event Action? OnAuthStateChanged;
        public event Action<RoleType?>? OnActiveRoleChanged;

        public bool IsAuthenticated { get; private set; } = false;
        public User? CurrentUser { get; private set; } = null;

        private async Task GetCurrentUser()
        {
            ServiceResponse<User> userResponse = await _userApi.GetUserAsync();
            if (userResponse.IsSuccess && userResponse.Response is not null)
            {
                CurrentUser = userResponse.Response;
                if (CurrentUser.Roles.Count == 1)
                    CurrentUser.Role = CurrentUser.Roles[0];
            }
        }

        public async Task InitializeAsync()
        {
            if (AppEnvironment.IsLogEnabled)
                _logger.LogInformation("Initializing auth service...");

            if (IsAuthenticated)
            {
                if (CurrentUser is null) await GetCurrentUser();
            }
            else
            {
                IsAuthenticated = (await _authApi.RefreshTokenAsync()).IsSuccess;
                if (IsAuthenticated) await GetCurrentUser();
            }

            OnAuthStateChanged?.Invoke();

            return;
        }

        public async Task<bool> LoginAsync(LoginModel request)
        {
            if (AppEnvironment.IsLogEnabled && _logger.IsEnabled(LogLevel.Information))
                _logger.LogInformation("Attempting login for email: {Email}", request.Email);

            if (!_commonAuthLogic.ValidateLoginModel(request))
                return false;

            var result = await _authApi.LoginAsync(request);
            if (result.IsSuccess)
            {
                IsAuthenticated = true;
                await GetCurrentUser();
                OnAuthStateChanged?.Invoke();
            }
            else
            {
                _globalNotificationService.ShowError(result.Message ?? "Вы указали неверные данные для входа. Попробуйте снова.");
                if (_logger.IsEnabled(LogLevel.Warning))
                    _logger.LogWarning("Login failed: {Error}", result.Message);
            }

            return result.IsSuccess;
        }

        public async Task LogoutAsync()
        {
            if (AppEnvironment.IsLogEnabled)
                _logger.LogInformation("Logging out user...");

            var result = await _authApi.LogoutAsync();
            if (result.IsSuccess)
            {
                IsAuthenticated = false;
                CurrentUser = null;
                OnAuthStateChanged?.Invoke();
                _navigationManager.NavigateTo("/login");
            }
            else
            {
                if (result.Message is not null) 
                    _globalNotificationService.ShowError(result.Message);
                if (_logger.IsEnabled(LogLevel.Warning))
                    _logger.LogWarning("Login failed: {Error}", result.Message);
            }
        }

        public async Task<Guid?> RequestPasswordRecoveryAsync(RecoveryModel recoveryModel)
        {
            if (AppEnvironment.IsLogEnabled && _logger.IsEnabled(LogLevel.Information))
                _logger.LogInformation("Attempting password recovery for email: {Email}", recoveryModel.Email);

            if (!_commonAuthLogic.ValidateRecoveryModel(recoveryModel))
                return null;

            var result = await _authApi.PasswordVerificationAsync(recoveryModel.Email);
            if (result.IsSuccess)
            {
                _globalNotificationService.ShowSuccess("Инструкции по восстановлению отправлены на email");
            }
            else
            {
                _globalNotificationService.ShowError(result.Message ?? "Не удалось отправить инструкцию по восстановлению на почту");
                if (_logger.IsEnabled(LogLevel.Warning))
                    _logger.LogWarning("Recovery password failed: {Error}", result.Message);
            }

            return result.Response;
        }

        public async Task<bool> ResetPasswordAsync(NewPasswordModel newPasswordModel)
        {
            if (AppEnvironment.IsLogEnabled)
                _logger.LogInformation("Attempting update password");

            if (!_commonAuthLogic.ValidateNewPasswordModel(newPasswordModel))
                return false;

            var result = await _authApi.PasswordNewAsync(newPasswordModel);
            if (result.IsSuccess)
            {
                _globalNotificationService.ShowSuccess("Пароль успешно изменен!");
            }
            else
            {
                _globalNotificationService.ShowError(result.Message ?? "Не удалось изменить пароль");
                if (_logger.IsEnabled(LogLevel.Warning))
                    _logger.LogWarning("Reset password failed: {Error}", result.Message);
            }

            return result.IsSuccess;
        }

        public async Task<bool> RegistrationAsync(RegisterModel request, Guid invitationId)
        {
            if (AppEnvironment.IsLogEnabled)
                _logger.LogInformation("Attempting user registration");

            if (!_commonAuthLogic.ValidateRegisterModel(request))
                return false;

            var result = await _authApi.RegistrationUserAsync(request, invitationId);
            if (result.IsSuccess)
            {
                IsAuthenticated = true;
                await GetCurrentUser();
                OnAuthStateChanged?.Invoke();
            }
            else
            {
                _globalNotificationService.ShowError(result.Message ?? "Вы указали неверные данные для регистрации. Попробуйте снова.");
                if (_logger.IsEnabled(LogLevel.Warning))
                    _logger.LogWarning("Registration failed: {Error}", result.Message);
            }

            return result.IsSuccess;
        }

        public async Task<bool> SetUserRoleAsync(RoleType roleType)
        {
            if (CurrentUser == null || !CurrentUser.Roles.Contains(roleType))
                return false;

            CurrentUser.Role = roleType;

            OnActiveRoleChanged?.Invoke(roleType);

            return true;
        }

        public async Task<bool> UpdateCurrentUser(
            string? email, string? firstName, string? lastName, List<RoleType>? roles, string? studyGroup, string? telephone
        )
        {
            if (CurrentUser is null) return false;

            if (email is not null) CurrentUser.Email = email;
            if (firstName is not null) CurrentUser.FirstName = firstName;
            if (lastName is not null) CurrentUser.LastName = lastName;
            if (roles is not null) CurrentUser.Roles = [.. roles];
            if (studyGroup is not null) CurrentUser.StudyGroup = studyGroup;
            if (telephone is not null) CurrentUser.Telephone = telephone;

            OnAuthStateChanged?.Invoke();

            return true;
        }
    }
}
