using HITSBlazor.Models.Common.Entities;
using HITSBlazor.Models.Ideas.Entities;
using HITSBlazor.Models.Ideas.Enums;
using HITSBlazor.Pages.Ideas.IdeasCreate;

namespace HITSBlazor.Services.Ideas
{
    public interface IIdeasService
    {
        //Ideas
        Task<List<Idea>> GetIdeasAsync(string? searchText = null, HashSet<IdeaStatusType>? statusTypes = null);
        Task<Idea?> GetIdeaByIdAsync(Guid id);
        Task<List<Skill>> GetAllIdeaSkillsAsync(Guid ideaId);
        Task<List<Rating>> GetIdeaRatingsAsync(Guid ideaId);
        Task<bool> CreateNewIdeaAsync(IdeasCreateModel ideasCreateModel);
        Task<bool> UpdateIdeaAsync(Guid ideaId, IdeasCreateModel ideasCreateModel);
        Task<bool> DeleteIdeaAsync(Idea idea);


        //Comments
        Task<List<Comment>> GetIdeasCommentsAsync(Guid ideaId);
        Task<bool> DeleteCommentInIdeaAsync(Comment comment);
    }
}
