using HITSBlazor.Models.Users.Entities;
using HITSBlazor.Models.Users.Enums;
using HITSBlazor.Pages.Login;
using HITSBlazor.Pages.NewPassword;
using HITSBlazor.Pages.Register;
using HITSBlazor.Utils;

namespace HITSBlazor.Services.Auth
{
    public interface IAuthService
    {
        event Action? OnAuthStateChanged;
        bool IsAuthenticated { get; }
        public User? CurrentUser { get; }

        Task InitializeAsync();
        Task<ServiceResponse<bool>> LoginAsync(LoginModel request);
        Task<ServiceResponse<bool>> LogoutAsync();
        Task<ServiceResponse<Guid>> RequestPasswordRecoveryAsync(string email);
        Task<ServiceResponse<bool>> ResetPasswordAsync(NewPasswordModel newPasswordModel);
        Task<ServiceResponse<bool>> RegistrationAsync(RegisterModel request, Guid invitationId);

        Task<bool> SetUserRoleAsync(RoleType roleType);
    }
}
