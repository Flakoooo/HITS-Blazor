using HITSBlazor.Models.Common.Entities;
using HITSBlazor.Utils.Mocks.Common;

namespace HITSBlazor.Services.UserSkills
{
    public class MockUserSkillService : IUserSkillService
    {
        public async Task<List<Skill>> GetUserSkillsAsync(Guid userId)
            => MockUsersSkills.GetUserSkillsById(userId) ?? [];

        public async Task UpdateUserSkillsAsync(Guid userId, List<Skill> skills)
        {
            MockUsersSkills.UpdateUserSkillsById(userId, skills);
        }
    }
}
