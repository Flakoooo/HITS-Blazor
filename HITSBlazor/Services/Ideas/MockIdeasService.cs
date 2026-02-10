using HITSBlazor.Models.Ideas.Entities;
using HITSBlazor.Models.Ideas.Enums;
using HITSBlazor.Pages.Ideas.IdeasCreate;
using HITSBlazor.Services.Auth;
using HITSBlazor.Utils;
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
            IdeaStatusType[]? statusTypes = null
        )
        {
            if (_cachedIdeas.Count == 0 || DateTime.UtcNow - _lastRefreshTime > _cacheLifetime)
                await RefreshCacheAsync();

            var query = _cachedIdeas.AsEnumerable();
            if (statusTypes != null && statusTypes.Length > 0)
                query = query.Where(i => statusTypes.Contains(i.Status));

            if (!string.IsNullOrWhiteSpace(searchText))
                query = query.Where(i => i.Name.Contains(searchText, StringComparison.CurrentCultureIgnoreCase));

            return [.. query];
        }

        public async Task<Idea?> GetIdeaByIdAsync(Guid id) => MockIdeas.GetIdeaById(id);

        public async Task<ServiceResponse<bool>> CreateNewIdea(IdeasCreateModel ideasCreateModel)
        {
            if (_authService.CurrentUser is null)
            {
                _globalNotificationService.ShowError("Пользователь не найден");
                return ServiceResponse<bool>.Failure("Пользователь не найден");
            }

            return ServiceResponse<bool>.Success(true);
        }
    }
}
