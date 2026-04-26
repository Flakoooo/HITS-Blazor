using HITSBlazor.Models.Common.Entities;
using HITSBlazor.Models.Common.Responses;
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

        public event Action<Tag>? OnTagHasCreated;
        public event Action<Tag>? OnTagHasUpdated;
        public event Action<Tag>? OnTagHasDeleted;

        public async Task<ListDataResponse<Tag>> GetTagsAsync(
            int page, string? searchText, bool? confirmed
        ) => MockTags.GetTags(page, searchText: searchText, confirmed: confirmed);

        public async Task<Tag?> CreateNewTagAsync(string name, string color, bool isConfirmed)
        {
            if (_authService.CurrentUser is null) return null;

            var newTag = MockTags.CreateTag(name, color, isConfirmed, _authService.CurrentUser.Id);
            if (newTag is not null)
                OnTagHasCreated?.Invoke(newTag);

            return newTag;
        }

        public async Task<bool> ConfirmTagAsync(Guid tagId)
        {
            if (_authService.CurrentUser is null) return false;

            var result = MockTags.ConfirmTag(tagId, _authService.CurrentUser.Id);
            if (result is null || result.Confirmed == false)
            {
                _globalNotificationService.ShowError("Не удалось утвердить тег");
                return false;
            }

            OnTagHasUpdated?.Invoke(result);
            _globalNotificationService.ShowSuccess("Тег успешно утвержден");
            return true;
        }

        public async Task<bool> UpdateTagAsync(Guid tagId, string name, string color)
        {
            if (_authService.CurrentUser is null) return false;

            var updatedTag = MockTags.UpdateTag(tagId, name, color, _authService.CurrentUser.Id);
            if (updatedTag is null)
            {
                _globalNotificationService.ShowError("Не удалось изменить тег");
                return false;
            }

            OnTagHasUpdated?.Invoke(updatedTag);
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

            OnTagHasDeleted?.Invoke(tag);
            _globalNotificationService.ShowSuccess("Тег успешно удален");
            return;
        }
    }
}
