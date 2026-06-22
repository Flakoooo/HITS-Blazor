using HITSBlazor.Models.Common.Entities;
using HITSBlazor.Models.Common.Requests;
using HITSBlazor.Models.Common.Responses;

namespace HITSBlazor.Services.Tags
{
    public class TagService(
        TagApi tagApi,
        ILogger<TagService> logger,
        GlobalNotificationService globalNotificationService
    ) : ITagService
    {
        private readonly TagApi _tagApi = tagApi;
        private readonly ILogger<TagService> _logger = logger;
        private readonly GlobalNotificationService _globalNotificationService = globalNotificationService;

        public event Action<Tag>? OnTagHasCreated;
        public event Action<Guid, UpdateTagRequest?, bool?>? OnTagHasUpdated;
        public event Action<Tag>? OnTagHasDeleted;

        public async Task<ListDataResponse<Tag>> GetTagsAsync(
            int page, string? searchText, bool? confirmed
        )
        {
            var result = await _tagApi.GetTagsAsync(page, searchText: searchText, confirmed: confirmed);
            if (result.IsSuccess && result.Response is not null)
                return result.Response;

            if (!string.IsNullOrWhiteSpace(result.Message))
            {
                _globalNotificationService.ShowError(result.Message);
                if (_logger.IsEnabled(LogLevel.Warning))
                    _logger.LogWarning("Get tags failed: {Error}", result.Message);
            }

            return new ListDataResponse<Tag>(0, []);
        }

        public async Task<Tag?> CreateNewTagAsync(string name, string color, bool isConfirmed)
        {
            var result = await _tagApi.CreateTagAsync(name, color, isConfirmed);
            if (result.IsSuccess && result.Response is not null)
            {
                OnTagHasCreated?.Invoke(result.Response);
                _globalNotificationService.ShowSuccess("Тег успешно создан!");
            }
            else if (!string.IsNullOrWhiteSpace(result.Message))
            {
                _globalNotificationService.ShowError(result.Message);
                if (_logger.IsEnabled(LogLevel.Warning))
                    _logger.LogWarning("Create tag failed: {Error}", result.Message);
            }

            return result.Response;
        }

        public async Task<bool> ConfirmTagAsync(Guid tagId)
        {
            var result = await _tagApi.ConfirmTagAsync(tagId);
            if (result.IsSuccess && result.Response is not null)
            {
                OnTagHasUpdated?.Invoke(tagId, null, true);
                _globalNotificationService.ShowSuccess("Тег успешно утвержден!");
                return true;
            }
            else if (!string.IsNullOrWhiteSpace(result.Message))
            {
                _globalNotificationService.ShowError(result.Message);
                if (_logger.IsEnabled(LogLevel.Warning))
                    _logger.LogWarning("Confirm tag failed: {Error}", result.Message);
            }

            return false;
        }

        public async Task<bool> UpdateTagAsync(Guid tagId, UpdateTagRequest request)
        {
            var result = await _tagApi.ConfirmTagAsync(tagId);
            if (result.IsSuccess && result.Response is not null)
            {
                OnTagHasUpdated?.Invoke(tagId, request, null);
                _globalNotificationService.ShowSuccess("Тег успешно обновлен!");
                return true;
            }
            else if (!string.IsNullOrWhiteSpace(result.Message))
            {
                _globalNotificationService.ShowError(result.Message);
                if (_logger.IsEnabled(LogLevel.Warning))
                    _logger.LogWarning("Update tag failed: {Error}", result.Message);
            }

            return false;
        }

        public async Task DeleteTagAsync(Tag tag)
        {
            var result = await _tagApi.DeleteTagAsync(tag.Id);
            if (result.IsSuccess && result.Response is not null)
            {
                OnTagHasDeleted?.Invoke(tag);
                _globalNotificationService.ShowSuccess("Тег успешно удален!");
            }
            else if (!string.IsNullOrWhiteSpace(result.Message))
            {
                _globalNotificationService.ShowError(result.Message);
                if (_logger.IsEnabled(LogLevel.Warning))
                    _logger.LogWarning("Delete tag failed: {Error}", result.Message);
            }
        }
    }
}
