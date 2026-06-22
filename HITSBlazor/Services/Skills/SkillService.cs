using HITSBlazor.Models.Common.Entities;
using HITSBlazor.Models.Common.Enums;
using HITSBlazor.Models.Common.Requests;
using HITSBlazor.Models.Common.Responses;

namespace HITSBlazor.Services.Skills
{
    public class SkillService(
        SkillApi skillApi,
        ILogger<SkillService> logger,
        GlobalNotificationService globalNotificationService
    ) : ISkillService
    {
        private readonly SkillApi _skillApi = skillApi;
        private readonly ILogger<SkillService> _logger = logger;
        private readonly GlobalNotificationService _globalNotificationService = globalNotificationService;

        public event Action<Skill>? OnSkillHasCreated;
        public event Action<UpdateSkillRequest>? OnSkillHasUpdated;
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
                _globalNotificationService.ShowSuccess("Компетенция успешно создана!");
            }
            else if (!string.IsNullOrWhiteSpace(result.Message))
            {
                _globalNotificationService.ShowError(result.Message);
                if (_logger.IsEnabled(LogLevel.Warning))
                    _logger.LogWarning("Create skill failed: {Error}", result.Message);
            }

            return result.Response;
        }

        public async Task<bool> UpdateSkillAsync(UpdateSkillRequest request)
        {
            var result = await _skillApi.UpdateSkillAsync(request);
            if (result.IsSuccess && result.Response is not null)
            {
                OnSkillHasUpdated?.Invoke(request);
                _globalNotificationService.ShowSuccess(result.Response);
                return true;
            }
            else if (!string.IsNullOrWhiteSpace(result.Message))
            {
                _globalNotificationService.ShowError(result.Message);
                if (_logger.IsEnabled(LogLevel.Warning))
                    _logger.LogWarning("Update skill failed: {Error}", result.Message);
            }

            return false;
        }

        public async Task DeleteSkillAsync(Skill skill)
        {
            var result = await _skillApi.DeleteSkillAsync(skill.Id);
            if (result.IsSuccess && result.Response is not null)
            {
                OnSkillHasDeleted?.Invoke(skill);
                _globalNotificationService.ShowSuccess(result.Response);
            }
            else if (!string.IsNullOrWhiteSpace(result.Message))
            {
                _globalNotificationService.ShowError(result.Message);
                if (_logger.IsEnabled(LogLevel.Warning))
                    _logger.LogWarning("Delete skill failed: {Error}", result.Message);
            }
        }
    }
}
