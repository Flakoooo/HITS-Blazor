using HITSBlazor.Models.Ideas.Entities;
using HITSBlazor.Utils.Mocks.Users;

namespace HITSBlazor.Utils.Mocks.Ideas
{
    public static class MockRatings
    {
        public static string Idea1Id { get; } = Guid.NewGuid().ToString();
        public static string Idea2Id { get; } = Guid.NewGuid().ToString();
        public static string Idea3Id { get; } = Guid.NewGuid().ToString();

        public static List<Rating> GetMockRatings()
        {
            var kirill = MockUsers.GetUserById(MockUsers.KirillId);
            var ivan = MockUsers.GetUserById(MockUsers.IvanId);

            return
            [
                new Rating
                {
                    Id = Idea1Id,
                    IdeaId = MockIdeas.RefactorId,
                    ExpertId = kirill.Id,
                    ExpertFirstName = kirill.FirstName,
                    ExpertLastName = kirill.LastName,
                    Budget = 1,
                    TechnicalRealizability = null,
                    Suitability = 2,
                    RatingValue = 8.0 / 5.0,
                    Originality = 3,
                    MarketValue = 1,
                    IsConfirmed = false
                },
                new Rating
                {
                    Id = Idea2Id,
                    IdeaId = MockIdeas.RefactorId,
                    ExpertId = ivan.Id,
                    ExpertFirstName = ivan.FirstName,
                    ExpertLastName = ivan.LastName,
                    Budget = 2,
                    TechnicalRealizability = 4,
                    Suitability = 5,
                    RatingValue = 14.0 / 5.0,
                    Originality = 1,
                    MarketValue = 2,
                    IsConfirmed = true
                },
                new Rating
                {
                    Id = Idea3Id,
                    IdeaId = MockIdeas.MyNewIdeaId,
                    ExpertId = kirill.Id,
                    ExpertFirstName = kirill.FirstName,
                    ExpertLastName = kirill.LastName,
                    Budget = 1,
                    TechnicalRealizability = 1,
                    Suitability = 5,
                    RatingValue = null,
                    Originality = null,
                    MarketValue = null,
                    IsConfirmed = false
                }
            ];
        }

        public static Rating? GetRatingById(string id)
            => GetMockRatings().FirstOrDefault(r => r.Id == id);

        public static List<Rating> GetRatingsByIdeaId(string ideaId)
            => [.. GetMockRatings().Where(r => r.IdeaId == ideaId)];

        public static List<Rating> GetRatingsByExpertId(string expertId)
            => [.. GetMockRatings().Where(r => r.ExpertId == expertId)];

        public static List<Rating> GetConfirmedRatings()
            => [.. GetMockRatings().Where(r => r.IsConfirmed)];

        public static List<Rating> GetUnconfirmedRatings()
            => [.. GetMockRatings().Where(r => !r.IsConfirmed)];

        public static int GetRatingsCountForIdea(string ideaId)
            => GetRatingsByIdeaId(ideaId).Count;

        public static bool HasExpertRatedIdea(string expertId, string ideaId)
            => GetMockRatings().Any(r => r.ExpertId == expertId && r.IdeaId == ideaId);
    }
}
