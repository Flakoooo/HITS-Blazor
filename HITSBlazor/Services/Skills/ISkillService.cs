using HITSBlazor.Models.Common.Entities;
using HITSBlazor.Models.Common.Enums;

namespace HITSBlazor.Services.Skills
{
    public interface ISkillService
    {
        Task<List<Skill>> GetSkillsAsync(string? searchText = null);

        Task<List<Skill>> GetSkillsByTypeAsync(SkillType skillType);
        Task<List<Skill>> GetSkillByTypeAndByNameAsync(SkillType skillType, string name);

        Task<Skill?> CreateNewSkillAsync(string name, SkillType type, bool isConfirmed);
    }
}
