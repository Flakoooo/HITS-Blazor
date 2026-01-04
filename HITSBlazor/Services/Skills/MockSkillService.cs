using HITSBlazor.Models.Common.Entities;
using HITSBlazor.Models.Common.Enums;
using HITSBlazor.Utils;
using HITSBlazor.Utils.Mocks.Common;

namespace HITSBlazor.Services.Skills
{
    public class MockSkillService : ISkillService
    {
        private List<Skill> _skills = [];

        public async Task<ServiceResponse<List<Skill>>> GetSkillsAsync()
        {
            _skills = MockSkills.GetAllSkills();

            return ServiceResponse<List<Skill>>.Success(_skills);
        }

        public async Task<List<Skill>> GetSkillsByTypeAsync(SkillType skillType)
        {
            if (_skills.Count == 0) await GetSkillsAsync();

            return [.. _skills.Where(s => s.Type == skillType)];
        }

        public async Task<List<Skill>> GetSkillByTypeAndByNameAsync(SkillType skillType, string name)
        {
            if (_skills.Count == 0) await GetSkillsAsync();

            return [.. _skills.Where(s => s.Type == skillType && s.Name.Contains(name, StringComparison.CurrentCultureIgnoreCase))]; ;
        }
    }
}
