using HITSBlazor.Models.Common.Entities;

namespace HITSBlazor.Services.UserSkills
{
    public interface IUserSkillService
    {
        Task<List<Skill>> GetUserSkillsAsync(Guid userId);
        Task UpdateUserSkillsAsync(Guid userId, List<Skill> skills);
    }
}
