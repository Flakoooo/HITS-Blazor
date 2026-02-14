using HITSBlazor.Models.Common.Entities;
using HITSBlazor.Models.Ideas.Entities;
using HITSBlazor.Models.Ideas.Enums;
using HITSBlazor.Pages.Ideas.IdeasCreate;
using HITSBlazor.Utils;

namespace HITSBlazor.Services.Ideas
{
    public interface IIdeasService
    {
        Task<List<Idea>> GetIdeasAsync(string? searchText = null, IdeaStatusType[]? statusTypes = null);
        Task<Idea?> GetIdeaByIdAsync(Guid id);
        Task<List<Skill>> GetAllIdeaSkillsAsync(Guid ideaId);
        Task<List<Rating>> GetIdeaRatingsAsync(Guid ideaId);
        Task<ServiceResponse<bool>> CreateNewIdea(IdeasCreateModel ideasCreateModel);


        //Comments
        Task<List<Comment>> GetIdeasCommentsAsync(Guid ideaId);
        Task<bool> DeleteCommentInIdeaAsync(Comment comment);
    }
}
