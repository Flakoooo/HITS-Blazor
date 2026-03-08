using HITSBlazor.Models.Common.Entities;
using HITSBlazor.Models.Common.Enums;
using HITSBlazor.Services.Auth;
using HITSBlazor.Utils.Mocks.Common;

namespace HITSBlazor.Services.Skills
{
    public class MockSkillService(IAuthService authService) : ISkillService
    {
        private readonly IAuthService _authService = authService;

        private List<Skill> _cachedSkills = [];
        private DateTime _lastRefreshTime;
        private readonly TimeSpan _cacheLifetime = TimeSpan.FromMinutes(5);

        private async Task RefreshCacheAsync()
        {
            _cachedSkills = MockSkills.GetAllSkills();
            _lastRefreshTime = DateTime.UtcNow;
        }

        public async Task<List<Skill>> GetSkillsAsync(
            string? searchText,
            SkillType? skillType
        )
        {
            if (_cachedSkills.Count == 0 || DateTime.UtcNow - _lastRefreshTime > _cacheLifetime)
                await RefreshCacheAsync();

            var query = _cachedSkills.AsEnumerable();

            if (skillType.HasValue)
                query = query.Where(s => s.Type == skillType);

            if (!string.IsNullOrWhiteSpace(searchText))
                query = query.Where(s => s.Name.Contains(searchText, StringComparison.CurrentCultureIgnoreCase));

            return [.. query];
        }

        public async Task<Skill?> CreateNewSkillAsync(string name, SkillType type, bool isConfirmed)
        {
            if (_authService.CurrentUser is null)
                return null;

            var newSkill = MockSkills.CreateSkill(name, type, isConfirmed, _authService.CurrentUser.Id);
            if (newSkill is not null)
            {
                if (!_cachedSkills.Contains(newSkill))
                    _cachedSkills.Add(newSkill);
            }

            return newSkill;
        }
    }
}
