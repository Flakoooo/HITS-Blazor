using HITSBlazor.Models.Common.Entities;
using HITSBlazor.Models.Common.Enums;
using HITSBlazor.Models.Common.Responses;
using HITSBlazor.Services.Auth;

namespace HITSBlazor.Services.Skills
{
    public class SkillService(
        IAuthService authService, 
        GlobalNotificationService globalNotificationService
    ) : ISkillService
    {
        private readonly IAuthService _authService = authService;
        private readonly GlobalNotificationService _globalNotificationService = globalNotificationService;

        public event Action<Skill>? OnSkillHasCreated;
        public event Action<Skill>? OnSkillHasUpdated;
        public event Action<Skill>? OnSkillHasDeleted;

        public async Task<ListDataResponse<Skill>> GetSkillsAsync(
            int page,
            string? searchText,
            bool? confirmed,
            IEnumerable<SkillType>? skillTypes
        )
        {
            _globalNotificationService.ShowError("Метод GetSkillsAsync не реализован");
            return new ListDataResponse<Skill>(0, []);
        }

        public async Task<Skill?> CreateNewSkillAsync(string name, SkillType type, bool isConfirmed)
        {
            _globalNotificationService.ShowError("Метод CreateNewSkillAsync не реализован");
            return null;
        }

        public async Task<bool> ConfirmSkillAsync(Guid skillId)
        {
            _globalNotificationService.ShowError("Метод ConfirmSkillAsync не реализован");
            return false;
        }

        public async Task<bool> UpdateSkillAsync(Guid skillId, string name, SkillType type)
        {
            _globalNotificationService.ShowError("Метод UpdateSkillAsync не реализован");
            return false;
        }

        public async Task DeleteSkillAsync(Skill skill)
        {
            _globalNotificationService.ShowError("Метод DeleteSkillAsync не реализован");
            return;
        }
    }
}
