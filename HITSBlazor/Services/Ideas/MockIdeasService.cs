using HITSBlazor.Models.Common.Entities;
using HITSBlazor.Models.Ideas.Entities;
using HITSBlazor.Models.Ideas.Enums;
using HITSBlazor.Pages.Ideas.IdeasCreate;
using HITSBlazor.Services.Auth;
using HITSBlazor.Utils.EnumTranslators;
using HITSBlazor.Utils.Mocks.Common;
using HITSBlazor.Utils.Mocks.Ideas;

namespace HITSBlazor.Services.Ideas
{
    public class MockIdeasService(IAuthService authService, GlobalNotificationService globalNotificationService) : IIdeasService
    {
        private readonly IAuthService _authService = authService;
        private readonly GlobalNotificationService _globalNotificationService = globalNotificationService;

        private List<Idea> _cachedIdeas = [];
        private DateTime _lastRefreshTime;
        private readonly TimeSpan _cacheLifetime = TimeSpan.FromMinutes(5);

        private async Task RefreshCacheAsync()
        {
            _cachedIdeas = MockIdeas.GetAllIdeas();
            _lastRefreshTime = DateTime.UtcNow;
        }

        public async Task<List<Idea>> GetIdeasAsync(
            string? searchText = null,
            HashSet<IdeaStatusType>? statusTypes = null
        )
        {
            if (_cachedIdeas.Count == 0 || DateTime.UtcNow - _lastRefreshTime > _cacheLifetime)
                await RefreshCacheAsync();

            var query = _cachedIdeas.AsEnumerable();
            if (statusTypes?.Count > 0)
                query = query.Where(i => statusTypes.Contains(i.Status));

            if (!string.IsNullOrWhiteSpace(searchText))
                query = query.Where(i => i.Name.Contains(searchText, StringComparison.CurrentCultureIgnoreCase));

            return [.. query];
        }

        public async Task<Idea?> GetIdeaByIdAsync(Guid id) => MockIdeas.GetIdeaById(id);

        public async Task<List<Skill>> GetAllIdeaSkillsAsync(Guid ideaId)
            => MockIdeaSkills.GetIdeaSkillsByIdeaId(ideaId);

        public async Task<List<Rating>> GetIdeaRatingsAsync(Guid ideaId)
            => MockRatings.GetIdeaRatingById(ideaId);

        public async Task<bool> CreateNewIdeaAsync(IdeasCreateModel ideasCreateModel)
        {
            if (_authService.CurrentUser is null)
            {
                _globalNotificationService.ShowError("Пользователь не найден");
                return false;
            }

            if (!MockIdeas.CreateNewIdea(ideasCreateModel, _authService.CurrentUser))
            {
                _globalNotificationService.ShowError("Не удалось создать идею");
                return false;
            }

            if (ideasCreateModel.Status == IdeaStatusType.New)
                _globalNotificationService.ShowSuccess("Черновик идеи сохранен");
            else if (ideasCreateModel.Status == IdeaStatusType.OnApproval)
                _globalNotificationService.ShowSuccess("Идея отправлена на согласование");

            return true;
        }

        public async Task<bool> UpdateIdeaAsync(Guid ideaId, IdeasCreateModel ideasCreateModel)
        {
            var idea = MockIdeas.GetIdeaById(ideaId);
            if (idea is null)
            {
                _globalNotificationService.ShowError("Редактируемая идея не найдена");
                return false;
            }

            if (!MockIdeas.UpdateIdea(ideaId, ideasCreateModel))
            {
                _globalNotificationService.ShowError("Не удалось обновить идею");
                return false;
            }

            _cachedIdeas = [];
            _lastRefreshTime = DateTime.MinValue;
            _globalNotificationService.ShowSuccess("Идея изменена");

            return true;
        }

        public async Task<bool> UpdateIdeaStatusAsync(Guid ideaId, IdeaStatusType ideaStatus)
        {
            var updatedIdea = MockIdeas.UpdateIdeaStatus(ideaId, ideaStatus);
            if (updatedIdea is null)
            {
                _globalNotificationService.ShowError($"Не удалось сменить статус идеи на {ideaStatus.GetTranslation()}");
                return false;
            }

            var index = _cachedIdeas.FindIndex(i => i.Id == ideaId);
            _cachedIdeas[index].Status = updatedIdea.Status;
            _cachedIdeas[index].ModifiedAt = updatedIdea.ModifiedAt;

            return true;
        }

        public async Task<bool> DeleteIdeaAsync(Idea idea)
        {
            if (!MockIdeas.DeleteIdea(idea))
            {
                _globalNotificationService.ShowError("Не удалось удалить идею");
                return false;
            }

            _cachedIdeas.Remove(idea);
            return true;
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
