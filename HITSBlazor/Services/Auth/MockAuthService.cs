using HITSBlazor.Models.Users.Entities;
using HITSBlazor.Models.Users.Enums;
using HITSBlazor.Pages.Login;
using HITSBlazor.Pages.NewPassword;
using HITSBlazor.Pages.Register;
using HITSBlazor.Utils;
using HITSBlazor.Utils.Mocks.Users;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System.Text.Json;

namespace HITSBlazor.Services.Auth
{
    public class MockAuthService(
        IJSRuntime jsRuntime, 
        NavigationManager navigationManager
    ) : IAuthService
    {
        private readonly IJSRuntime _jsRuntime = jsRuntime;
        private readonly NavigationManager _navigationManager = navigationManager;
        private readonly string _mockTokenTemplate = "mock-token-";

        public event Action? OnAuthStateChanged;

        public bool IsAuthenticated { get; private set; }
        public User? CurrentUser { get; private set; } = null;

        public async Task InitializeAsync()
        {
            var token = await GetTokenAsync();
            IsAuthenticated = !string.IsNullOrEmpty(token) 
                && Guid.TryParse(token.Skip(_mockTokenTemplate.Length).ToString(), out var _);

            OnAuthStateChanged?.Invoke();
        }

        public async Task<ServiceResponse<bool>> LoginAsync(LoginModel request)
        {
            try
            {
                var user = MockUsers.GetAllUsers()
                    .FirstOrDefault(u => u.Email == request.Email);

                if (user == null)
                    return ServiceResponse<bool>.Failure("Неверный логин или пароль");

                var mockToken = _mockTokenTemplate + user.Id;

                await SaveTokenAsync(mockToken);

                IsAuthenticated = true;
                CurrentUser = user;

                await SaveUserInfoAsync(user);

                OnAuthStateChanged?.Invoke();

                return ServiceResponse<bool>.Success(true);
            }
            catch (Exception ex)
            {
                return ServiceResponse<bool>.Failure($"Ошибка при авторизации: {ex.Message}");
            }
        }

        public async Task<ServiceResponse<bool>> LogoutAsync()
        {
            await RemoveTokenAsync();
            await RemoveUserInfoAsync();
            IsAuthenticated = false;
            OnAuthStateChanged?.Invoke();
            _navigationManager.NavigateTo("/login");
            return ServiceResponse<bool>.Success(true);
        }

        public async Task<ServiceResponse<bool>> RegistrationAsync(RegisterModel request, Guid invitationId)
        {
            try
            {
                if (!MockUsers.GetAllUsers().Select(u => u.Email).Contains(request.Email))
                    return ServiceResponse<bool>.Failure("Пользователь с таким email уже существует");

                return ServiceResponse<bool>.Success(true, "Регистрация успешна");
            }
            catch (Exception ex)
            {
                return ServiceResponse<bool>.Failure($"Ошибка: {ex.Message}");
            }
        }

        public async Task<ServiceResponse<Guid>> RequestPasswordRecoveryAsync(string email)
        {
            try
            {
                if (!MockUsers.GetAllUsers().Select(u => u.Email).Contains(email))
                    return ServiceResponse<Guid>.Failure("Пользователь с таким email не найден");

                return ServiceResponse<Guid>.Success(Guid.NewGuid(), "Инструкции по восстановлению отправлены на email");
            }
            catch (Exception ex)
            {
                return ServiceResponse<Guid>.Failure($"Ошибка: {ex.Message}");
            }
        }

        public async Task<ServiceResponse<bool>> ResetPasswordAsync(NewPasswordModel newPasswordModel)
        {
            try
            {
                if (newPasswordModel.Code != "123456")
                    return ServiceResponse<bool>.Failure("Указан неверный код");

                return ServiceResponse<bool>.Success(true, "Пароль успешно изменен!");
            }
            catch (Exception ex)
            {
                return ServiceResponse<bool>.Failure($"Ошибка: {ex.Message}");
            }
        }

        public bool SetUserRoleAsync(RoleType roleType)
        {
            if (CurrentUser == null || !CurrentUser.Roles.Contains(roleType))
                return false;

            CurrentUser.Role = roleType;

            OnAuthStateChanged?.Invoke();

            return true;
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
