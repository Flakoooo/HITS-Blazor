using HITSBlazor.Models.Auth.Requests;
using HITSBlazor.Models.Auth.Response;
using HITSBlazor.Models.Users.Entities;
using HITSBlazor.Utils.Mocks.Users;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System.Linq;
using System.Text.Json;

namespace HITSBlazor.Services
{
    public class MockAuthService(IJSRuntime jsRuntime, NavigationManager navigationManager) : IAuthService
    {
        private readonly IJSRuntime _jsRuntime = jsRuntime;
        private readonly NavigationManager _navigationManager = navigationManager;
        private readonly string _mockTokenTemplate = "mock-token-";

        public event Action? OnAuthStateChanged;

        public User? CurrentUser { get; private set; }
        public bool IsAuthenticated => CurrentUser != null;

        public async Task InitializeAsync()
        {
            var token = await GetTokenAsync();
            if (!string.IsNullOrEmpty(token))
            {
                var user = MockUsers.GetUserById(token.Skip(_mockTokenTemplate.Length).ToString() ?? string.Empty);
                CurrentUser = user is not null ? user : null;
            }
            else
            {
                CurrentUser = null;
            }

            OnAuthStateChanged?.Invoke();
        }

        public async Task<LoginResponse> LoginAsync(LoginRequest request)
        {
            try
            {
                var user = MockUsers.GetAllUsers()
                    .FirstOrDefault(u => u.Email == request.Email);

                if (user == null)
                    return LoginResponse.Failure("Неверный логин или пароль");

                var mockToken = _mockTokenTemplate + user.Id;

                await SaveTokenAsync(mockToken);

                CurrentUser = user;

                await SaveUserInfoAsync(user);

                OnAuthStateChanged?.Invoke();

                return LoginResponse.Success(mockToken, user);
            }
            catch (Exception ex)
            {
                return LoginResponse.Failure($"Ошибка при авторизации: {ex.Message}");
            }
        }

        public async Task LogoutAsync()
        {
            await RemoveTokenAsync();
            await RemoveUserInfoAsync();
            CurrentUser = null;
            OnAuthStateChanged?.Invoke();
            _navigationManager.NavigateTo("/login", true);
        }

        public async Task<RegisterResponse> RegisterAsync(RegisterRequest request, string? invitationCode = null)
        {
            try
            {
                if (!MockUsers.GetAllUsers().Select(u => u.Email).Contains(request.Email))
                    return RegisterResponse.Failure("Пользователь с таким email уже существует");

                return RegisterResponse.Success("Регистрация успешна");
            }
            catch (Exception ex)
            {
                return RegisterResponse.Failure($"Ошибка: {ex.Message}");
            }
        }

        public async Task<RecoveryResponse> RequestPasswordRecoveryAsync(string email)
        {
            try
            {
                if (!MockUsers.GetAllUsers().Select(u => u.Email).Contains(email))
                    return RecoveryResponse.Failure("Пользователь с таким email не найден");

                return RecoveryResponse.Success("Инструкции по восстановлению отправлены на email");
            }
            catch (Exception ex)
            {
                return RecoveryResponse.Failure($"Ошибка: {ex.Message}");
            }
        }

        public async Task<ResetPasswordResponse> ResetPasswordAsync(ResetPasswordRequest resetPassword)
        {
            try
            {
                if (resetPassword.Code != "123456")
                    return ResetPasswordResponse.Failure("Указан неверный код");

                return ResetPasswordResponse.Success("Пароль успешно изменен!");
            }
            catch (Exception ex)
            {
                return ResetPasswordResponse.Failure($"Ошибка: {ex.Message}");
            }
        }

        private async Task SaveTokenAsync(string token)
        {
            await _jsRuntime.InvokeVoidAsync("localStorage.setItem", "authToken", token);
        }

        private async Task<string?> GetTokenAsync()
        {
            return await _jsRuntime.InvokeAsync<string?>("localStorage.getItem", "authToken");
        }

        private async Task RemoveTokenAsync()
        {
            await _jsRuntime.InvokeVoidAsync("localStorage.removeItem", "authToken");
        }

        private async Task SaveUserInfoAsync(User user)
        {
            var userJson = JsonSerializer.Serialize(user);
            await _jsRuntime.InvokeVoidAsync("localStorage.setItem", "userInfo", userJson);
        }

        private async Task RemoveUserInfoAsync()
        {
            await _jsRuntime.InvokeVoidAsync("localStorage.removeItem", "userInfo");
        }
    }
}
