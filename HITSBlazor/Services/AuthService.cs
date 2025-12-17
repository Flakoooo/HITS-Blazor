using HITSBlazor.Models.Auth.Requests;
using HITSBlazor.Models.Auth.Response;
using HITSBlazor.Models.Users.Entities;
using HITSBlazor.Utils.Mocks.Users;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System.Text.Json;

namespace HITSBlazor.Services
{
    public class AuthService(
        HttpClient httpClient, IJSRuntime jsRuntime, NavigationManager navigationManager
    ) : IAuthService
    {
        private readonly HttpClient _httpClient = httpClient;
        private readonly IJSRuntime _jsRuntime = jsRuntime;
        private readonly NavigationManager _navigationManager = navigationManager;

        public event Action? OnAuthStateChanged;

        public User? CurrentUser { get; private set; }
        public bool IsAuthenticated => CurrentUser != null;

        public async Task InitializeAsync()
        {
            var token = await GetTokenAsync();
            if (!string.IsNullOrEmpty(token))
            {
                await LoadMockUserAsync();
            }
            else
            {
                CurrentUser = null;
                OnAuthStateChanged?.Invoke();
            }
        }

        private async Task LoadMockUserAsync()
        {
            var mockUser = MockUsers.GetUserById(MockUsers.KirillId);
            if (mockUser != null)
            {
                CurrentUser = mockUser;
                OnAuthStateChanged?.Invoke();
            }
        }

        public async Task<LoginResponse> LoginAsync(LoginRequest request)
        {
            try
            {
                var user = MockUsers.GetAllUsers()
                    .FirstOrDefault(u => u.Email == request.Email);

                if (user == null)
                    return LoginResponse.Failure("Неверный логин или пароль");

                var mockToken = $"mock-token-{user.Id}";

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

        public async Task<RecoveryResponse> RequestPasswordRecoveryAsync(string email)
        {
            try
            {
                await Task.Delay(1000);

                return RecoveryResponse.Success("Инструкции по восстановлению отправлены на email");
            }
            catch (Exception ex)
            {
                return RecoveryResponse.Failure($"Ошибка: {ex.Message}");
            }
        }

        public async Task<ResetPasswordResponse> ResetPasswordAsync(string code, string newPassword)
        {
            try
            {
                // TODO: Заменить на реальный вызов API
                // var response = await _httpClient.PostAsJsonAsync("/api/auth/reset-password", 
                //     new { Code = code, NewPassword = newPassword });

                await Task.Delay(1000); // Имитация задержки

                // Простая проверка кода (для демо)
                if (code != "123456") // В реальном приложении код будет приходить на email
                {
                    return new ResetPasswordResponse
                    {
                        Success = false,
                        Message = "Неверный код подтверждения"
                    };
                }

                // Проверка сложности пароля
                if (newPassword.Length < 8)
                {
                    return new ResetPasswordResponse
                    {
                        Success = false,
                        Message = "Пароль должен содержать минимум 8 символов"
                    };
                }

                return new ResetPasswordResponse
                {
                    Success = true,
                    Message = "Пароль успешно изменен"
                };
            }
            catch (Exception ex)
            {
                return new ResetPasswordResponse
                {
                    Success = false,
                    Message = $"Ошибка: {ex.Message}"
                };
            }
        }

        public async Task<RegisterResponse> RegisterAsync(RegisterRequest request, string? invitationCode = null)
        {
            try
            {
                // TODO: Заменить на реальный вызов API
                // var response = await _httpClient.PostAsJsonAsync("/api/auth/register", 
                //     new { request, InvitationCode = invitationCode });

                await Task.Delay(1000); // Имитация задержки

                // Проверка кода приглашения (если требуется)
                if (!string.IsNullOrEmpty(invitationCode) && invitationCode != "valid-invitation-code")
                {
                    return new RegisterResponse
                    {
                        Success = false,
                        Message = "Неверный код приглашения"
                    };
                }

                // Проверка существующего email
                var existingUser = MockUsers.GetAllUsers()
                    .FirstOrDefault(u => u.Email == request.Email);

                if (existingUser != null)
                {
                    return new RegisterResponse
                    {
                        Success = false,
                        Message = "Пользователь с таким email уже существует"
                    };
                }

                return new RegisterResponse
                {
                    Success = true,
                    Message = "Регистрация успешна"
                };
            }
            catch (Exception ex)
            {
                return new RegisterResponse
                {
                    Success = false,
                    Message = $"Ошибка: {ex.Message}"
                };
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
