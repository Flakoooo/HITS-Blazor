using HITSBlazor.Models.Common.Entities;
using HITSBlazor.Services.Auth;
using HITSBlazor.Utils.Mocks.Common;

namespace HITSBlazor.Services.Tags
{
    public class MockTagService(
        IAuthService authService,
        GlobalNotificationService globalNotificationService
    ) : ITagService
    {
        private readonly IAuthService _authService = authService;
        private readonly GlobalNotificationService _globalNotificationService = globalNotificationService;

        public event Func<Task>? OnTagsStateChanged;
        public event Action? OnTagsStateUpdated;

        private List<Tag> _cachedTags = [];
        private DateTime _lastRefreshTime;
        private readonly TimeSpan _cacheLifetime = TimeSpan.FromMinutes(5);

        private async Task RefreshCacheAsync()
        {
            _cachedTags = MockTags.GetTags();
            _lastRefreshTime = DateTime.UtcNow;
        }

        public async Task<List<Tag>> GetTagsAsync(string? searchText, bool? confirmed)
        {
            if (_cachedTags.Count == 0 || DateTime.UtcNow - _lastRefreshTime > _cacheLifetime)
                await RefreshCacheAsync();

            var query = _cachedTags.AsEnumerable();

            if (confirmed.HasValue)
                query = query.Where(t => t.Confirmed == confirmed.Value);

            if (!string.IsNullOrWhiteSpace(searchText))
                query = query.Where(t => t.Name.Contains(searchText, StringComparison.CurrentCultureIgnoreCase));

            return [.. query];
        }

        public async Task<Tag?> CreateNewTagAsync(string name, string color, bool isConfirmed)
        {
            if (_authService.CurrentUser is null)
                return null;

            var newTag = MockTags.CreateTag(name, color, isConfirmed, _authService.CurrentUser.Id);
            if (newTag is not null)
            {
                if (!_cachedTags.Contains(newTag))
                    _cachedTags.Add(newTag);
            }

            OnTagsStateChanged?.Invoke();
            return newTag;
        }

        public async Task<bool> ConfirmTagAsync(Guid tagId)
        {
            if (_authService.CurrentUser is null)
                return false;

            var result = MockTags.ConfirmTag(tagId, _authService.CurrentUser.Id);
            if (!result)
            {
                _globalNotificationService.ShowError("Не удалось утвердить тег");
                return false;
            }

            var tagforUpdate = _cachedTags.FirstOrDefault(t => t.Id == tagId);
            if (tagforUpdate is not null)
            {
                tagforUpdate.Confirmed = true;
                tagforUpdate.UpdaterId = _authService.CurrentUser.Id;
            }

            OnTagsStateUpdated?.Invoke();
            _globalNotificationService.ShowSuccess("Тег успешно утвержден");
            return true;
        }

        public async Task<bool> UpdateTagAsync(Guid tagId, string name, string color)
        {
            if (_authService.CurrentUser is null)
                return false;

            var updatedTag = MockTags.UpdateTag(tagId, name, color, _authService.CurrentUser.Id);
            if (updatedTag is null)
            {
                _globalNotificationService.ShowError("Не удалось изменить тег");
                return false;
            }

            var tagForUpdate = _cachedTags.FirstOrDefault(t => t.Id == tagId);
            if (tagForUpdate is not null)
            {
                tagForUpdate.Name = name;
                tagForUpdate.Color = color;
                tagForUpdate.UpdaterId = _authService.CurrentUser.Id;
            }

            OnTagsStateUpdated?.Invoke();
            _globalNotificationService.ShowSuccess("Тег успешно изменен");
            return true;
        }

        public async Task DeleteTagAsync(Tag tag)
        {
            if (!MockTags.DeleteTag(tag))
            {
                _globalNotificationService.ShowError("Не удалось удалить тег");
                return;
            }

            _cachedTags.Remove(tag);
            OnTagsStateChanged?.Invoke();
            _globalNotificationService.ShowSuccess("Тег успешно удален");
            return;
        }
    }
}
