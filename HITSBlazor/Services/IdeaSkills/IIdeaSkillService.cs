using HITSBlazor.Models.Common.Entities;

namespace HITSBlazor.Services.IdeaSkills
{
    public interface IIdeaSkillService
    {
        Task<List<Skill>> GetAllIdeaSkillsAsync(Guid ideaId);
    }
}
