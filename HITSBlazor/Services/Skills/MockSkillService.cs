using HITSBlazor.Models.Common.Entities;
using HITSBlazor.Models.Common.Enums;
using HITSBlazor.Models.Common.Requests;
using HITSBlazor.Models.Common.Responses;
using HITSBlazor.Services.Auth;
using HITSBlazor.Utils.Mocks.Common;

namespace HITSBlazor.Services.Skills
{
    public class MockSkillService(
        IAuthService authService, 
        GlobalNotificationService globalNotificationService
    ) : ISkillService
    {
        private readonly IAuthService _authService = authService;
        private readonly GlobalNotificationService _globalNotificationService = globalNotificationService;

        public event Action<Skill>? OnSkillHasCreated;
        public event Action<UpdateSkillRequest>? OnSkillHasUpdated;
        public event Action<Skill>? OnSkillHasDeleted;

        public async Task<ListDataResponse<Skill>> GetSkillsAsync(
            int page,
            string? searchText,
            bool? confirmed,
            IEnumerable<SkillType>? skillTypes
        ) => MockSkills.GetAllSkills(
            page, searchText: searchText, confirmed: confirmed, skillTypes: skillTypes?.ToHashSet()
        );

        public async Task<Skill?> CreateNewSkillAsync(string name, SkillType type, bool isConfirmed)
        {
            if (_authService.CurrentUser is null) return null;

            var newSkill = MockSkills.CreateSkill(name, type, isConfirmed, _authService.CurrentUser.Id);
            if (newSkill is not null)
                OnSkillHasCreated?.Invoke(newSkill);

            return newSkill;
        }

        public async Task<bool> UpdateSkillAsync(UpdateSkillRequest request)
        {
            if (_authService.CurrentUser is null)
                return false;

            var updatedSkill = MockSkills.UpdateSkill(request, _authService.CurrentUser.Id);
            if (updatedSkill is null)
            {
                _globalNotificationService.ShowError("Не удалось изменить компетенцию");
                return false;
            }

            OnSkillHasUpdated?.Invoke(request);
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

            OnSkillHasDeleted?.Invoke(skill);
            _globalNotificationService.ShowSuccess("Компетенция успешно удалена");
            return;
        }
    }
}
