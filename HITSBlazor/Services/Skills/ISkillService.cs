using HITSBlazor.Models.Common.Entities;
using HITSBlazor.Models.Common.Enums;
using HITSBlazor.Utils;

namespace HITSBlazor.Services.Skills
{
    public interface ISkillService
    {
        Task<ServiceResponse<List<Skill>>> GetSkillsAsync();

        Task<List<Skill>> GetSkillsByTypeAsync(SkillType skillType);
        Task<List<Skill>> GetSkillByTypeAndByNameAsync(SkillType skillType, string name);
    }
}
