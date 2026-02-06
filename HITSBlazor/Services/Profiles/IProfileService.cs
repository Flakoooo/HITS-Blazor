using HITSBlazor.Components.Modals.ShowUserModal;
using HITSBlazor.Models.Users.Entities;

namespace HITSBlazor.Services.Profiles
{
    public interface IProfileService
    {
        Task<Profile?> GetUserProifleAsync(Guid userId);
        Task<bool> UpdateProfileUserData(UserDataForm userDataForm);
    }
}
