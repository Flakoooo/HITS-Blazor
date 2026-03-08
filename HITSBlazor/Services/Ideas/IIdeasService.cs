using HITSBlazor.Components.Modals.RightSideModals.IdeaModal;
using HITSBlazor.Models.Common.Entities;
using HITSBlazor.Models.Ideas.Entities;
using HITSBlazor.Models.Ideas.Enums;
using HITSBlazor.Pages.Ideas.IdeasCreate;

namespace HITSBlazor.Services.Ideas
{
    public interface IIdeasService
    {
        event Action? OnIdeasStateChanged;

        //Ideas
        Task<List<Idea>> GetIdeasAsync(
            IdeasQueryType queryType, 
            string? searchText = null, 
            HashSet<IdeaStatusType>? statusTypes = null
        );
        Task<Idea?> GetIdeaByIdAsync(Guid id);
        Task<Idea?> CreateNewIdeaAsync(IdeasCreateModel ideasCreateModel);
        Task<bool> UpdateCheckedIdeaAsync(Guid ideaId);
        Task<bool> UpdateIdeaAsync(Guid ideaId, IdeasCreateModel ideasCreateModel);
        Task<bool> UpdateIdeaStatusAsync(Guid ideaId, IdeaStatusType ideaStatus);
        Task<bool> DeleteIdeaAsync(Idea idea);

        //Skills
        Task<List<Skill>> GetAllIdeaSkillsAsync(Guid ideaId);
        Task CreateOrUpdateIdeasSkills(Guid ideaId, List<Skill> skills);

        //Ratings
        Task<List<Rating>> GetIdeaRatingsAsync(Guid ideaId);
        Task<bool> SendRatingAsync(RatingRequest request, bool isConfirmed = false);

        //Comments
        Task<List<Comment>> GetIdeasCommentsAsync(Guid ideaId);
        Task<bool> DeleteCommentInIdeaAsync(Comment comment);
    }
}
