using HITSBlazor.Models.Ideas.Entities;

namespace HITSBlazor.Services.IdeaRatings
{
    public interface IIdeaRatingService
    {
        Task<List<Rating>> GetIdeaRatingsAsync(Guid ideaId);
    }
}
