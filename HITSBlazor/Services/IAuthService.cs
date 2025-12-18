using HITSBlazor.Models.Auth.Requests;
using HITSBlazor.Models.Auth.Response;
using HITSBlazor.Models.Users.Entities;

namespace HITSBlazor.Services
{
    public interface IAuthService
    {
        event Action? OnAuthStateChanged;
        User? CurrentUser { get; }
        bool IsAuthenticated { get; }

        Task InitializeAsync();
        Task<LoginResponse> LoginAsync(LoginRequest request);
        Task<RecoveryResponse> RequestPasswordRecoveryAsync(string email);
        Task<ResetPasswordResponse> ResetPasswordAsync(ResetPasswordRequest resetPassword);
        Task<RegisterResponse> RegisterAsync(RegisterRequest request, string? invitationCode = null);
        Task LogoutAsync();
    }
}
