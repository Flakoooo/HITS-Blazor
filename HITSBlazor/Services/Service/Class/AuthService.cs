using HITSBlazor.Models.Auth.Requests;
using HITSBlazor.Models.Auth.Response;
using HITSBlazor.Services.Api;
using HITSBlazor.Services.Service.Interfaces;
using Microsoft.AspNetCore.Components;

namespace HITSBlazor.Services.Service.Class
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

        public async Task InitializeAsync()
        {
            _logger.LogInformation("Initializing auth service...");

            IsAuthenticated = (await _authApi.RefreshTokenAsync()).IsSuccess;

            OnAuthStateChanged?.Invoke();
            return;
        }

        public async Task<LoginResponse> LoginAsync(LoginRequest request)
        {
            _logger.LogInformation("Attempting login for email: {Email}", request.Email);

            var result = await _authApi.LoginAsync(request);
            if (result.IsSuccess)
            {
                IsAuthenticated = true;
                OnAuthStateChanged?.Invoke();
                return LoginResponse.Success();
            }

            _logger.LogWarning("Login failed: {Error}", result.Message);
            return LoginResponse.Failure(result.Message ?? "Ошибка входа");
        }

        public async Task LogoutAsync()
        {
            _logger.LogInformation("Logging out user...");

            //TODO: Сделать Logout с бэка

            IsAuthenticated = false;

            _navigationManager.NavigateTo("/login");
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
