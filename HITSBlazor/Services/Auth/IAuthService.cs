using HITSBlazor.Models.Users.Entities;
using HITSBlazor.Models.Users.Enums;
using HITSBlazor.Pages.Login;
using HITSBlazor.Pages.NewPassword;
using HITSBlazor.Pages.RecoveryPassword;
using HITSBlazor.Pages.Register;

namespace HITSBlazor.Services.Auth
{
    public interface IAuthService
    {
        event Action? OnAuthStateChanged;
        bool IsAuthenticated { get; }
        public User? CurrentUser { get; }

        Task InitializeAsync();
        Task<bool> LoginAsync(LoginModel request);
        Task LogoutAsync();
        Task<Guid?> RequestPasswordRecoveryAsync(RecoveryModel recoveryModel);
        Task<bool> ResetPasswordAsync(NewPasswordModel newPasswordModel);
        Task<bool> RegistrationAsync(RegisterModel request, Guid invitationId);

        Task<bool> SetUserRoleAsync(RoleType roleType);
    }
}
