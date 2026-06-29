using HITSBlazor.Components.Modals.RightSideModals.ProfileModal;
using HITSBlazor.Models.Users.Entities;
using Microsoft.AspNetCore.Components.Forms;

namespace HITSBlazor.Services.Profiles
{
    public interface IProfileService
    {
        event Action<string?>? OnUserAvatarHasChanged;

        Task<Profile?> GetUserProifleAsync(Guid userId);
        Task<string?> GetUserProifleAvatarAsync(Guid userId, bool refresh = false);
        Task<bool> UpdateProfileUserDataAsync(UserDataForm userDataForm);
        Task<bool> UpdateProfileAvatarAsync(IBrowserFile avatar);
        Task<Guid> SendUpdateEmailRequestAsync(string email);
        Task<bool> UpdateEmailConfirmAsync(Guid updateVerificationCode, string code);
    }
}
