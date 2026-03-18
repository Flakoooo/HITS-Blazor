using HITSBlazor.Models.Common.Entities;

namespace HITSBlazor.Services.Tags
{
    public interface ITagService
    {
        event Func<Task>? OnTagsStateChanged;
        event Action? OnTagsStateUpdated;

        Task<List<Tag>> GetTagsAsync(
            string? searchText = null,
            bool? confirmed = null
        );

        Task<Tag?> CreateNewTagAsync(string name, string color, bool isConfirmed);
        Task<bool> ConfirmTagAsync(Guid tagId);
        Task<bool> UpdateTagAsync(Guid tagId, string name, string color);
        Task DeleteTagAsync(Tag tag);
    }
}
