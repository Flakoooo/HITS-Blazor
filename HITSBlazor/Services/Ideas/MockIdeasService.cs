using HITSBlazor.Components.Modals.RightSideModals.IdeaModal;
using HITSBlazor.Models.Common.Entities;
using HITSBlazor.Models.Ideas.Entities;
using HITSBlazor.Models.Ideas.Enums;
using HITSBlazor.Models.Users.Enums;
using HITSBlazor.Pages.Ideas.IdeasCreate;
using HITSBlazor.Services.Auth;
using HITSBlazor.Utils;
using HITSBlazor.Utils.EnumTranslators;
using HITSBlazor.Utils.Mocks.Common;
using HITSBlazor.Utils.Mocks.Ideas;

namespace HITSBlazor.Services.Ideas
{
    public class MockIdeasService(IAuthService authService, GlobalNotificationService globalNotificationService) : IIdeasService
    {
        private readonly IAuthService _authService = authService;
        private readonly GlobalNotificationService _globalNotificationService = globalNotificationService;

        private readonly Dictionary<IdeasQueryType, CacheEntry<List<Idea>>> _cache = [];
        private readonly TimeSpan _cacheLifetime = TimeSpan.FromMinutes(5);

        public event Action? OnIdeasStateChanged;

        private List<Idea> GetCurrentIdeas(IdeasQueryType queryType)
        {
            return queryType switch
            {
                IdeasQueryType.All => MockIdeas.GetAllIdeas(),
                IdeasQueryType.Initiator => _authService.CurrentUser is not null
                    ? MockIdeas.GetIdeasByInitiatorId(_authService.CurrentUser.Id)
                    : [],
                IdeasQueryType.OnConfirmation => MockIdeas.GetIdeasOnCofirmation(),
                _ => [],
            };
        }

        private async Task<bool> CanAccessAsync(IdeasQueryType queryType) 
            => _authService.CurrentUser is not null && queryType switch 
            {
                IdeasQueryType.All => true,
                IdeasQueryType.Initiator => _authService.CurrentUser.Role is RoleType.Initiator or RoleType.Admin,
                IdeasQueryType.OnConfirmation => _authService.CurrentUser.Role is RoleType.Expert or RoleType.Admin,
                _ => false
            };

        public async Task<List<Idea>> GetIdeasAsync(
            IdeasQueryType queryType,
            string? searchText = null,
            HashSet<IdeaStatusType>? statusTypes = null
        )
        {
            if (!await CanAccessAsync(queryType)) return [];

            List<Idea> ideas;

            if (_cache.TryGetValue(queryType, out var cache) && !cache.IsExpired(_cacheLifetime))
            {
                ideas = cache.Data;
            }
            else
            {
                ideas = GetCurrentIdeas(queryType);
                _cache[queryType] = new CacheEntry<List<Idea>>(ideas);
            }

            var query = ideas.AsEnumerable();

            if (statusTypes?.Count > 0)
                query = query.Where(i => statusTypes.Contains(i.Status));

            if (!string.IsNullOrWhiteSpace(searchText))
                query = query.Where(i => i.Name.Contains(searchText, StringComparison.CurrentCultureIgnoreCase));

            return [.. query];
        }

        public async Task<Idea?> GetIdeaByIdAsync(Guid id) => MockIdeas.GetIdeaById(id);

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

            _cache.Clear();

            return true;
        }

        public async Task<bool> UpdateCheckedIdeaAsync(Guid ideaId)
        {
            if (!MockIdeas.CheckIdea(ideaId)) return false;

            foreach (var cache in _cache.Values)
            {
                var index = cache.Data.FindIndex(i => i.Id == ideaId);
                if (index >= 0) cache.Data[index].IsChecked = true;
            }

            OnIdeasStateChanged?.Invoke();
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

            _cache.Clear();
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

            foreach (var cache in _cache)
            {
                var index = cache.Value.Data.FindIndex(i => i.Id == ideaId);
                if (index >= 0)
                {
                    cache.Value.Data[index].Status = updatedIdea.Status;
                    cache.Value.Data[index].ModifiedAt = updatedIdea.ModifiedAt;
                }
            }

            OnIdeasStateChanged?.Invoke();
            return true;
        }

        public async Task<bool> DeleteIdeaAsync(Idea idea)
        {
            if (!MockIdeas.DeleteIdea(idea))
            {
                _globalNotificationService.ShowError("Не удалось удалить идею");
                return false;
            }

            foreach (var cache in _cache) cache.Value.Data.Remove(idea);

            return true;
        }

        //Skills
        public async Task<List<Skill>> GetAllIdeaSkillsAsync(Guid ideaId)
            => MockIdeaSkills.GetIdeaSkillsByIdeaId(ideaId);

        //Ratings
        public async Task<List<Rating>> GetIdeaRatingsAsync(Guid ideaId)
            => MockRatings.GetIdeaRatingById(ideaId);

        public async Task<bool> SendRatingAsync(RatingRequest request, bool isConfirmed)
        {
            if (isConfirmed)
            {
                if (!MockRatings.UpdateOrConfirmRating(request, isConfirmed))
                {
                    _globalNotificationService.ShowError("Не удалось подтвердить рейтинг");
                    return false;
                }

                _globalNotificationService.ShowSuccess("Рейтинг успешно подтвержден");
            }
            else
            {
                if (!MockRatings.UpdateOrConfirmRating(request))
                {
                    _globalNotificationService.ShowError("Не удалось сохранить рейтинг");
                    return false;
                }

                _globalNotificationService.ShowSuccess("Рейтинг успешно сохранен");
            }

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
