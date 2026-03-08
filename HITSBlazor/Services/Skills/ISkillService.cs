using HITSBlazor.Models.Common.Entities;
using HITSBlazor.Models.Common.Enums;

namespace HITSBlazor.Services.Skills
{
    public interface ISkillService
    {
        Task<List<Skill>> GetSkillsAsync(
            string? searchText = null,
            SkillType? skillType = null
        );

        Task<Skill?> CreateNewSkillAsync(string name, SkillType type, bool isConfirmed);
    }
}
