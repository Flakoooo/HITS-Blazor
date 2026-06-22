using HITSBlazor.Models.Common.Entities;
using HITSBlazor.Models.Common.Requests;
using HITSBlazor.Models.Common.Responses;

namespace HITSBlazor.Services.Tags
{
    public interface ITagService
    {
        event Action<Tag>? OnTagHasCreated;
        event Action<Guid, UpdateTagRequest?, bool?>? OnTagHasUpdated;
        event Action<Tag>? OnTagHasDeleted;

        Task<ListDataResponse<Tag>> GetTagsAsync(
            int page, string? searchText = null, bool? confirmed = null
        );

        Task<Tag?> CreateNewTagAsync(string name, string color, bool isConfirmed);
        Task<bool> ConfirmTagAsync(Guid tagId);
        Task<bool> UpdateTagAsync(Guid tagId, UpdateTagRequest request);
        Task DeleteTagAsync(Tag tag);
    }
}
