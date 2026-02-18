using HITSBlazor.Models.Common.Entities;
using HITSBlazor.Models.Common.Enums;
using HITSBlazor.Utils;

namespace HITSBlazor.Services.Skills
{
    public interface ISkillService
    {
        Task<List<Skill>> GetSkillsAsync(string? searchText = null);

        Task<List<Skill>> GetSkillsByTypeAsync(SkillType skillType);
        Task<List<Skill>> GetSkillByTypeAndByNameAsync(SkillType skillType, string name);
    }
}
