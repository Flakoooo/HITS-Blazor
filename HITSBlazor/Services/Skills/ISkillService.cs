using HITSBlazor.Models.Common.Entities;
using HITSBlazor.Models.Common.Enums;

namespace HITSBlazor.Services.Skills
{
    public interface ISkillService
    {
        event Func<Task>? OnSkillsStateChanged;
        event Action? OnSkillsStateUpdated;

        Task<List<Skill>> GetSkillsAsync(
            string? searchText = null,
            bool? confirmed = null,
            HashSet<SkillType>? skillTypes = null
        );

        Task<Skill?> CreateNewSkillAsync(string name, SkillType type, bool isConfirmed);
        Task<bool> ConfirmSkillAsync(Guid skillId);
        Task<bool> UpdateSkillAsync(Guid skillId, string name, SkillType type);
        Task DeleteSkillAsync(Skill skill);
    }
}
