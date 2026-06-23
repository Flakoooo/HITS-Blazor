using HITSBlazor.Models.Common.Entities;
using HITSBlazor.Models.Common.Responses;
using HITSBlazor.Models.Ideas.Entities;
using HITSBlazor.Models.Ideas.Enums;
using HITSBlazor.Pages.Ideas.IdeasCreate;

namespace HITSBlazor.Services.Ideas
{
    public interface IIdeasService
    {
        event Action<Idea>? OnIdeaHasDeleted;
        event Action<Guid, bool>? OnIdeaHasOpened;
        event Action<Guid, IdeaStatusType>? OnIdeasStatusHasChanged;

        //Ideas
        Task<ListDataResponse<Idea>> GetIdeasAsync(
            int page,
            string? searchText = null,
            HashSet<IdeaStatusType>? statusTypes = null
        );
        Task<Idea?> GetIdeaByIdAsync(Guid id);
        Task<Idea?> CreateNewIdeaAsync(IdeasCreateModel ideasCreateModel);
        Task<bool> UpdateIdeaAsync(IdeasCreateModel ideasCreateModel);
        Task<bool> UpdateIdeaStatusAsync(Guid ideaId, IdeaStatusType ideaStatus);
        Task<bool> DeleteIdeaAsync(Idea idea);

        void IdeasStatusHasUpdatedEvent(Guid ideaId, IdeaStatusType ideaStatus);

        //Skills
        Task<List<Skill>> GetAllIdeaSkillsAsync(Guid ideaId);
        Task CreateOrUpdateIdeasSkills(Guid ideaId, List<Skill> skills);

        //Comments
        Task<List<Comment>> GetIdeasCommentsAsync(Guid ideaId);
        Task<bool> DeleteCommentInIdeaAsync(Comment comment);
    }
}
