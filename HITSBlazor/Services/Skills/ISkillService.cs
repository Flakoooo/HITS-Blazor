using HITSBlazor.Models.Common.Entities;
using HITSBlazor.Models.Common.Enums;
using HITSBlazor.Models.Common.Responses;

namespace HITSBlazor.Services.Skills
{
    public interface ISkillService
    {
        event Action<Skill>? OnSkillHasCreated;
        event Action<Skill>? OnSkillHasUpdated;
        event Action<Skill>? OnSkillHasDeleted;

        Task<ListDataResponse<Skill>> GetSkillsAsync(
            int page,
            string? searchText = null,
            bool? confirmed = null,
            IEnumerable<SkillType>? skillTypes = null
        );

        Task<Skill?> CreateNewSkillAsync(string name, SkillType type, bool isConfirmed);
        Task<bool> ConfirmSkillAsync(Guid skillId);
        Task<bool> UpdateSkillAsync(Guid skillId, string name, SkillType type);
        Task DeleteSkillAsync(Skill skill);
    }
}
