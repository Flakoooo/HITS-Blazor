using HITSBlazor.Components.Modals.RightSideModals.IdeaModal;
using HITSBlazor.Models.Ideas.Entities;
using HITSBlazor.Models.Ideas.Enums;
using HITSBlazor.Utils.Models;

namespace HITSBlazor.Utils.Mocks.Ideas
{
    public static class MockRatings
    {
        private static readonly List<Rating> _ratings = [];

        public static Guid Idea1Id { get; } = Guid.NewGuid();
        public static Guid Idea2Id { get; } = Guid.NewGuid();
        public static Guid Idea3Id { get; } = Guid.NewGuid();

        private static readonly Random _random = new();
        private static int CreateExpertOptionValue() => _random.Next(1, 6);

        public static List<Rating> GetIdeaRatingById(Guid ideaId)
            => [.. _ratings.Where(r => r.IdeaId == ideaId)];

        public static Rating? GetRatingById(Guid id) => _ratings.FirstOrDefault(r => r.Id == id);

        public static double? CreateRatingByIdea(Idea idea)
        {
            double expertsRating = 0.0;
            bool ideaState = idea.Status == IdeaStatusType.Confirmed || idea.Status == IdeaStatusType.OnMarket;
            foreach (var expert in idea.Experts!.Users)
            {
                var rating = new Rating
                {
                    Id = Guid.NewGuid(),
                    IdeaId = idea.Id,
                    ExpertId = expert.Id,
                    ExpertFirstName = expert.FirstName,
                    ExpertLastName = expert.LastName,
                    Budget = ideaState ? CreateExpertOptionValue() : null,
                    TechnicalRealizability = ideaState ? CreateExpertOptionValue() : null,
                    Suitability = ideaState ? CreateExpertOptionValue() : null,
                    Originality = ideaState ? CreateExpertOptionValue() : null,
                    MarketValue = ideaState ? CreateExpertOptionValue() : null,
                    IsConfirmed = ideaState
                };

                if (
                    rating.Budget.HasValue && 
                    rating.Suitability.HasValue && 
                    rating.TechnicalRealizability.HasValue &&
                    rating.Originality.HasValue &&
                    rating.MarketValue.HasValue
                )
                {
                    rating.RatingValue = ideaState ? Formulas.CalculcateRating(
                        [
                            rating.Budget.Value,
                            rating.Suitability.Value,
                            rating.TechnicalRealizability.Value,
                            rating.Originality.Value,
                            rating.MarketValue.Value
                        ]
                    ) : null;
                }
                if (ideaState && rating.RatingValue.HasValue) expertsRating += rating.RatingValue.Value;

                _ratings.Add(rating);
            }

            return expertsRating == 0.0 ? null : expertsRating / idea.Experts!.Users.Count;
        }

        public static bool UpdateOrConfirmRating(RatingRequest request, bool isConfirmed = false)
        {
            if (!request.Id.HasValue) return false;

            var rating = GetRatingById(request.Id.Value);
            if (rating is null) return false;

            rating.MarketValue = request.MarketValue;
            rating.Originality = request.Originality;
            rating.TechnicalRealizability = request.TechnicalRealizability;
            rating.Suitability = request.Suitability;
            rating.Budget = request.Budget;

            if (
                rating.Budget.HasValue &&
                rating.Suitability.HasValue &&
                rating.TechnicalRealizability.HasValue &&
                rating.Originality.HasValue &&
                rating.MarketValue.HasValue
            )
            {
                rating.RatingValue = Formulas.CalculcateRating(
                    [
                        rating.Budget.Value,
                        rating.Suitability.Value,
                        rating.TechnicalRealizability.Value,
                        rating.Originality.Value,
                        rating.MarketValue.Value
                    ]
                );
            }

            if (isConfirmed)
            {
                rating.IsConfirmed = true;
                var allIdeaRatings = _ratings.Where(r => r.IdeaId == rating.IdeaId);
                if (allIdeaRatings.Count() == allIdeaRatings.Count(r => r.IsConfirmed))
                    MockIdeas.GetIdeaById(rating.IdeaId)?.Status = IdeaStatusType.Confirmed;
            }    

            return true;
        }
    }
}
