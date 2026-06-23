using HITSBlazor.Models.Common.Entities;
using HITSBlazor.Models.Common.Responses;
using HITSBlazor.Models.Ideas.Entities;
using HITSBlazor.Models.Ideas.Enums;
using HITSBlazor.Models.Users.Entities;
using HITSBlazor.Models.Users.Enums;
using HITSBlazor.Pages.Ideas.IdeasCreate;
using HITSBlazor.Utils.EnumUIConverters;
using HITSBlazor.Utils.Mocks.Common;
using HITSBlazor.Utils.Mocks.Ideas;

namespace HITSBlazor.Services.Ideas
{
    public class IdeasService(
        IdeaApi ideaApi,
        ILogger<IdeasService> logger,
        GlobalNotificationService globalNotificationService
    ) : IIdeasService
    {
        private readonly IdeaApi _ideaApi = ideaApi;
        private readonly ILogger<IdeasService> _logger = logger;
        private readonly GlobalNotificationService _globalNotificationService = globalNotificationService;

        public event Action<Idea>? OnIdeaHasDeleted;
        public event Action<Guid, bool>? OnIdeaHasOpened;
        public event Action<Guid, IdeaStatusType>? OnIdeasStatusHasChanged;

        public void IdeasStatusHasUpdatedEvent(Guid ideaId, IdeaStatusType ideaStatus)
            => OnIdeasStatusHasChanged?.Invoke(ideaId, ideaStatus);

        public async Task<ListDataResponse<Idea>> GetIdeasAsync(
            int page,
            string? searchText,
            HashSet<IdeaStatusType>? statusTypes
        )
        {
            var result = await _ideaApi.GetIdeasAsync(page, searchText: searchText, statusTypes: statusTypes);
            if (result.IsSuccess && result.Response is not null)
                return result.Response;

            if (!string.IsNullOrWhiteSpace(result.Message))
            {
                _globalNotificationService.ShowError(result.Message);
                if (_logger.IsEnabled(LogLevel.Warning))
                    _logger.LogWarning("Get ideas failed: {Error}", result.Message);
            }

            return new ListDataResponse<Idea>(0, []);
        }

        public async Task<Idea?> GetIdeaByIdAsync(Guid id)
        {
            var result = await _ideaApi.GetIdeaAsync(id);
            if (result.IsSuccess && result.Response is not null)
            {
                OnIdeaHasOpened?.Invoke(id, true);
            }
            else if (!string.IsNullOrWhiteSpace(result.Message))
            {
                _globalNotificationService.ShowError(result.Message);
                if (_logger.IsEnabled(LogLevel.Warning))
                    _logger.LogWarning("Get idea failed: {Error}", result.Message);
            }

            return result.Response;
        }

        public async Task<Idea?> CreateNewIdeaAsync(IdeasCreateModel ideasCreateModel)
        {
            var result = await _ideaApi.CreateIdeaAsync(ideasCreateModel);
            if (result.IsSuccess && result.Response is not null)
            {
                if (ideasCreateModel.Status == IdeaStatusType.New)
                    _globalNotificationService.ShowSuccess("Черновик идеи сохранен");
                else if (ideasCreateModel.Status == IdeaStatusType.OnApproval)
                    _globalNotificationService.ShowSuccess("Идея отправлена на согласование");
            }
            else if (!string.IsNullOrWhiteSpace(result.Message))
            {
                _globalNotificationService.ShowError(result.Message);
                if (_logger.IsEnabled(LogLevel.Warning))
                    _logger.LogWarning("Get idea failed: {Error}", result.Message);
            }

            return result.Response;
        }

        public async Task<bool> UpdateIdeaAsync(IdeasCreateModel ideasCreateModel)
        {
            var result = await _ideaApi.UpdateIdeaAsync(ideasCreateModel);
            if (result.IsSuccess && result.Response is not null)
            {
                _globalNotificationService.ShowSuccess("Идея изменена");
                return true;
            }
            else if (!string.IsNullOrWhiteSpace(result.Message))
            {
                _globalNotificationService.ShowError(result.Message);
                if (_logger.IsEnabled(LogLevel.Warning))
                    _logger.LogWarning("Get idea failed: {Error}", result.Message);
            }

            return false;
        }

        public async Task<bool> UpdateIdeaStatusAsync(Guid ideaId, IdeaStatusType ideaStatus)
        {
            var result = await _ideaApi.UpdateIdeaStatusAsync(ideaId, ideaStatus);
            if (result.IsSuccess && result.Response is not null)
            {
                OnIdeasStatusHasChanged?.Invoke(ideaId, ideaStatus);
                _globalNotificationService.ShowSuccess(result.Response);
                return true;
            }
            else if (!string.IsNullOrWhiteSpace(result.Message))
            {
                _globalNotificationService.ShowError(result.Message);
                if (_logger.IsEnabled(LogLevel.Warning))
                    _logger.LogWarning("Update idea status failed: {Error}", result.Message);
            }

            return false;
        }

        public async Task<bool> DeleteIdeaAsync(Idea idea)
        {
            var result = await _ideaApi.DeleteIdeaAsync(idea.Id);
            if (result.IsSuccess && result.Response is not null)
            {
                OnIdeaHasDeleted?.Invoke(idea);
                _globalNotificationService.ShowSuccess(result.Response);
                return true;
            }
            else if (!string.IsNullOrWhiteSpace(result.Message))
            {
                _globalNotificationService.ShowError(result.Message);
                if (_logger.IsEnabled(LogLevel.Warning))
                    _logger.LogWarning("Delete idea failed: {Error}", result.Message);
            }

            return false;
        }

        //Skills
        public async Task<List<Skill>> GetAllIdeaSkillsAsync(Guid ideaId)
        {
            var result = await _ideaApi.GetIdeasSkillsAsync(ideaId);
            if (result.IsSuccess && result.Response is not null)
                return result.Response;

            if (!string.IsNullOrWhiteSpace(result.Message))
            {
                _globalNotificationService.ShowError(result.Message);
                if (_logger.IsEnabled(LogLevel.Warning))
                    _logger.LogWarning("Get ideas skills failed: {Error}", result.Message);
            }

            return [];
        }

        public async Task CreateOrUpdateIdeasSkills(Guid ideaId, List<Skill> skills)
        {
            var result = await _ideaApi.UpdateIdeasSkillsAsync(ideaId, skills);
            if (result.IsSuccess && result.Response is not null)
            {
                _globalNotificationService.ShowSuccess("Компетенции идеи успешно изменены");
            }
            else if (!string.IsNullOrWhiteSpace(result.Message))
            {
                _globalNotificationService.ShowError(result.Message);
                if (_logger.IsEnabled(LogLevel.Warning))
                    _logger.LogWarning("Update idea skills failed: {Error}", result.Message);
            }
        }

        //Comments
        public async Task<List<Comment>> GetIdeasCommentsAsync(Guid ideaId)
            => MockComments.GetIdeasCommentsByIdeaId(ideaId);

        public async Task<bool> DeleteCommentInIdeaAsync(Comment comment)
        {
            if (!MockComments.DeleteIdeasComment(comment))
            {
                _globalNotificationService.ShowError("Не удалось удалить комментарий");
                return false;
            }

            return true;
        }
    }
}
