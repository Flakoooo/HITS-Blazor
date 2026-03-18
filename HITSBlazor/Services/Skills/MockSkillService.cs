using HITSBlazor.Models.Common.Entities;
using HITSBlazor.Models.Common.Enums;
using HITSBlazor.Services.Auth;
using HITSBlazor.Utils.Mocks.Common;
using System.ComponentModel.Design;
using System.Xml.Linq;

namespace HITSBlazor.Services.Skills
{
    public class MockSkillService(
        IAuthService authService, 
        GlobalNotificationService globalNotificationService
    ) : ISkillService
    {
        private readonly IAuthService _authService = authService;
        private readonly GlobalNotificationService _globalNotificationService = globalNotificationService;

        public event Func<Task>? OnSkillsStateChanged;
        public event Action? OnSkillsStateUpdated;

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
            HashSet<SkillType>? skillTypes
        )
        {
            if (_cachedSkills.Count == 0 || DateTime.UtcNow - _lastRefreshTime > _cacheLifetime)
                await RefreshCacheAsync();

            var query = _cachedSkills.AsEnumerable();

            if (skillTypes?.Count > 0)
                query = query.Where(s => skillTypes.Contains(s.Type));

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

            OnSkillsStateChanged?.Invoke();
            return newSkill;
        }

        public async Task<bool> ConfirmSkillAsync(Guid skillId)
        {
            if (_authService.CurrentUser is null)
                return false;

            var result = MockSkills.ConfirmSkill(skillId, _authService.CurrentUser.Id);
            if (!result)
            {
                _globalNotificationService.ShowError("Не удалось утвердить компетенцию");
                return false;
            }

            var skillForUpdate = _cachedSkills.FirstOrDefault(u => u.Id == skillId);
            if (skillForUpdate is not null)
            {
                skillForUpdate.Confirmed = true;
                skillForUpdate.UpdaterId = _authService.CurrentUser.Id;
            }

            OnSkillsStateUpdated?.Invoke();
            _globalNotificationService.ShowSuccess("Компетенция успешно утверждена");
            return true;
        }

        public async Task<bool> UpdateSkillAsync(Guid skillId, string name, SkillType type)
        {
            if (_authService.CurrentUser is null)
                return false;

            var updatedSkill = MockSkills.UpdateSkill(skillId, name, type, _authService.CurrentUser.Id);
            if (updatedSkill is null)
            {
                _globalNotificationService.ShowError("Не удалось изменить компетенцию");
                return false;
            }

            var skillForUpdate = _cachedSkills.FirstOrDefault(u => u.Id == skillId);
            if (skillForUpdate is not null)
            {
                skillForUpdate.Name = name;
                skillForUpdate.Type = type;
                skillForUpdate.UpdaterId = _authService.CurrentUser.Id;
            }

            OnSkillsStateUpdated?.Invoke();
            _globalNotificationService.ShowSuccess("Компетенция успешно изменена");
            return true;
        }

        public async Task DeleteSkillAsync(Skill skill)
        {
            if (!MockSkills.DeleteSkill(skill))
            {
                _globalNotificationService.ShowError("Не удалось удалить компетенцию");
                return;
            }

            _cachedSkills.Remove(skill);
            OnSkillsStateChanged?.Invoke();
            _globalNotificationService.ShowSuccess("Компетенция успешно удалена");
            return;
        }
    }
}
