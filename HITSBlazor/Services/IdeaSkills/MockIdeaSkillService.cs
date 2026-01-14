using HITSBlazor.Models.Common.Entities;
using HITSBlazor.Utils.Mocks.Ideas;

namespace HITSBlazor.Services.IdeaSkills
{
    public class MockIdeaSkillService : IIdeaSkillService
    {
        public async Task<List<Skill>> GetAllIdeaSkillsAsync(Guid ideaId)
            => MockIdeaSkills.GetIdeaSkillsByIdeaId(ideaId);
    }
}
