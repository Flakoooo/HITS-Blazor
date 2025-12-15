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
    )
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
                    return new LoginResponse
                    {
                        Success = false,
                        ErrorMessage = "Неверный логин или пароль"
                    };

                var mockToken = $"mock-token-{Guid.NewGuid()}";

                await SaveTokenAsync(mockToken);

                CurrentUser = user;

                await SaveUserInfoAsync(user);

                OnAuthStateChanged?.Invoke();

                return new LoginResponse
                {
                    Success = true,
                    Token = mockToken,
                    User = user
                };
            }
            catch (Exception ex)
            {
                return new LoginResponse
                {
                    Success = false,
                    ErrorMessage = $"Ошибка при авторизации: {ex.Message}"
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
