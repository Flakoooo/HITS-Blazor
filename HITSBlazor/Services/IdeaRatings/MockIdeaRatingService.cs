using HITSBlazor.Models.Ideas.Entities;
using HITSBlazor.Utils.Mocks.Ideas;

namespace HITSBlazor.Services.IdeaRatings
{
    public class MockIdeaRatingService : IIdeaRatingService
    {
        public async Task<List<Rating>> GetIdeaRatingsAsync(Guid ideaId)
            => MockRatings.GetIdeaRatingById(ideaId);
    }
}
