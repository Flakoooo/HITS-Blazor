using HITSBlazor.Models.Common.Entities;
using HITSBlazor.Models.Common.Enums;
using HITSBlazor.Models.Common.Requests;
using HITSBlazor.Models.Common.Responses;

namespace HITSBlazor.Services.Skills
{
    public interface ISkillService
    {
        event Action<Skill>? OnSkillHasCreated;
        event Action<UpdateSkillRequest>? OnSkillHasUpdated;
        event Action<Skill>? OnSkillHasDeleted;

        Task<ListDataResponse<Skill>> GetSkillsAsync(
            int page,
            string? searchText = null,
            bool? confirmed = null,
            IEnumerable<SkillType>? skillTypes = null
        );

        Task<Skill?> CreateNewSkillAsync(string name, SkillType type, bool isConfirmed);
        Task<bool> UpdateSkillAsync(UpdateSkillRequest request);
        Task DeleteSkillAsync(Skill skill);
    }
}
