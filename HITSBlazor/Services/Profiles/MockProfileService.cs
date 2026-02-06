using HITSBlazor.Components.Modals.RightSideModals.ShowUserModal;
using HITSBlazor.Models.Users.Entities;
using HITSBlazor.Services.Auth;
using HITSBlazor.Utils.Mocks.Users;

namespace HITSBlazor.Services.Profiles
{
    public class MockProfileService(IAuthService authService) : IProfileService
    {
        private readonly IAuthService _authService = authService;

        public async Task<Profile?> GetUserProifleAsync(Guid userId)
            => MockProfiles.GetUserProfileByUserId(userId);

        public async Task<bool> UpdateProfileUserData(UserDataForm userDataForm) 
            => await _authService.UpdateCurrentUser(
                firstName: userDataForm.FirstName,
                lastName: userDataForm.LastName,
                studyGroup: userDataForm.StudyGroup,
                telephone: userDataForm.Telephone
            );
    }
}
