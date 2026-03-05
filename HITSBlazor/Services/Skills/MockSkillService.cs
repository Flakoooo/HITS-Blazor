using HITSBlazor.Models.Common.Entities;
using HITSBlazor.Models.Common.Enums;
using HITSBlazor.Utils.Mocks.Common;

namespace HITSBlazor.Services.Skills
{
    public class MockSkillService : ISkillService
    {
        private List<Skill> _cachedSkills = [];
        private DateTime _lastRefreshTime;
        private readonly TimeSpan _cacheLifetime = TimeSpan.FromMinutes(5);

        private async Task RefreshCacheAsync()
        {
            _cachedSkills = MockSkills.GetAllSkills();
            _lastRefreshTime = DateTime.UtcNow;
        }

        public async Task<List<Skill>> GetSkillsAsync(
            string? searchText = null
        )
        {
            if (_cachedSkills.Count == 0 || DateTime.UtcNow - _lastRefreshTime > _cacheLifetime)
                await RefreshCacheAsync();

            var query = _cachedSkills.AsEnumerable();

            if (!string.IsNullOrWhiteSpace(searchText))
                query = query.Where(i => i.Name.Contains(searchText, StringComparison.CurrentCultureIgnoreCase));

            return [.. query];
        }

        public async Task<List<Skill>> GetSkillsByTypeAsync(SkillType skillType)
        {
            if (_cachedSkills.Count == 0 || DateTime.UtcNow - _lastRefreshTime > _cacheLifetime)
                await RefreshCacheAsync();

            return [.. _cachedSkills.Where(s => s.Type == skillType)];
        }

        public async Task<List<Skill>> GetSkillByTypeAndByNameAsync(SkillType skillType, string name)
        {
            if (_cachedSkills.Count == 0 || DateTime.UtcNow - _lastRefreshTime > _cacheLifetime)
                await RefreshCacheAsync();

            return [.. _cachedSkills.Where(s => s.Type == skillType && s.Name.Contains(name, StringComparison.CurrentCultureIgnoreCase))];
        }
    }
}
