using HITSBlazor.Models.Users.Entities;
using HITSBlazor.Utils.Mocks.Users;

namespace HITSBlazor.Services.Profiles
{
    public class MockProfileService : IProfileService
    {
        public async Task<Profile?> GetUserProifleAsync(Guid userId)
            => MockProfiles.GetUserProfileByUserId(userId);
    }
}
