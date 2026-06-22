using HITSBlazor.Models.Common.Entities;
using HITSBlazor.Models.Common.Enums;
using HITSBlazor.Models.Common.Responses;
using HITSBlazor.Services.Auth;
using System.Xml.Linq;

namespace HITSBlazor.Services.Skills
{
    public class SkillService(
        IAuthService authService, 
        SkillApi skillApi,
        ILogger<SkillService> logger,
        GlobalNotificationService globalNotificationService
    ) : ISkillService
    {
        private readonly IAuthService _authService = authService;
        private readonly SkillApi _skillApi = skillApi;
        private readonly ILogger<SkillService> _logger = logger;
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
            var result = await _skillApi.GetSkillsAsync(
                page,
                searchText: searchText,
                confirmed: confirmed,
                skillTypes: skillTypes
            );

            if (result.IsSuccess && result.Response is not null)
                return result.Response;

            if (!string.IsNullOrWhiteSpace(result.Message))
            {
                _globalNotificationService.ShowError(result.Message);
                if (_logger.IsEnabled(LogLevel.Warning))
                    _logger.LogWarning("Get skills failed: {Error}", result.Message);
            }

            return new ListDataResponse<Skill>(0, []);
        }

        public async Task<Skill?> CreateNewSkillAsync(string name, SkillType type, bool isConfirmed)
        {
            var result = await _skillApi.CreateSkillAsync(name, type);
            if (result.IsSuccess && result.Response is not null)
            {
                OnSkillHasCreated?.Invoke(result.Response);
            }
            else if (!string.IsNullOrWhiteSpace(result.Message))
            {
                _globalNotificationService.ShowError(result.Message);
                if (_logger.IsEnabled(LogLevel.Warning))
                    _logger.LogWarning("Create skill failed: {Error}", result.Message);
            }

            return result.Response;
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
            var result = await _skillApi.DeleteSkillAsync(skill.Id);
            if (result.IsSuccess && result.Response is not null)
            {
                OnSkillHasDeleted?.Invoke(skill);
                _globalNotificationService.ShowError(result.Response);
            }
            else if (!string.IsNullOrWhiteSpace(result.Message))
            {
                _globalNotificationService.ShowError(result.Message);
                if (_logger.IsEnabled(LogLevel.Warning))
                    _logger.LogWarning("Delete skill failed: {Error}", result.Message);
            }

            return;
        }
    }
}
