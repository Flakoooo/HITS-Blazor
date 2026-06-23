using HITSBlazor.Models.Common.Entities;

namespace HITSBlazor.Services.UserSkills
{
    public class UserSkillService(
        UserSkillsApi userSkillsApi, 
        ILogger<UserSkillService> logger,
        GlobalNotificationService globalNotificationService
    ) : IUserSkillService
    {
        private readonly UserSkillsApi _userSkillsApi = userSkillsApi;
        private readonly ILogger<UserSkillService> _logger = logger;
        private readonly GlobalNotificationService _globalNotificationService = globalNotificationService;

        public async Task<List<Skill>> GetUserSkillsAsync(Guid userId)
        {
            var result = await _userSkillsApi.GetUserSkillsAsync(userId);
            if (result.IsSuccess && result.Response is not null)
                return result.Response;

            if (!string.IsNullOrWhiteSpace(result.Message))
            {
                _globalNotificationService.ShowError(result.Message);
                if (_logger.IsEnabled(LogLevel.Warning))
                    _logger.LogWarning("Get user skills failed: {Error}", result.Message);
            }

            return [];
        }

        public async Task UpdateUserSkillsAsync(Guid userId, List<Skill> skills)
        {
            var result = await _userSkillsApi.UpdateUserSkillsAsync(skills.Select(s => s.Id));
            if (result.IsSuccess && result.Response is not null)
            {
                _globalNotificationService.ShowSuccess(result.Response);
            }
            else if (!string.IsNullOrWhiteSpace(result.Message))
            {
                _globalNotificationService.ShowError(result.Message);
                if (_logger.IsEnabled(LogLevel.Warning))
                    _logger.LogWarning("Update user skills failed: {Error}", result.Message);
            }
        }
    }
}
