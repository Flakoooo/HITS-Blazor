using HITSBlazor.Components.Modals.RightSideModals.IdeaModal;
using HITSBlazor.Models.Ideas.Entities;

namespace HITSBlazor.Services.Ratings
{
    public interface IRatingService
    {
        Task<List<Rating>> GetIdeaRatingsAsync(Guid ideaId);
        Task<bool> SendRatingAsync(RatingRequest request, bool isConfirmed = false, List<Rating>? ideasRatings = null);
    }
}
