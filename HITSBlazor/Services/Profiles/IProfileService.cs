using HITSBlazor.Components.Modals.RightSideModals.ShowUserModal;
using HITSBlazor.Models.Users.Entities;

namespace HITSBlazor.Services.Profiles
{
    public interface IProfileService
    {
        Task<Profile?> GetUserProifleAsync(Guid userId);
        Task<bool> UpdateProfileUserDataAsync(UserDataForm userDataForm);
        Task<Guid> SendUpdateEmailRequestAsync(string email);
        Task<bool> UpdateEmailConfirmAsync(Guid updateVerificationCode, string code);
    }
}
