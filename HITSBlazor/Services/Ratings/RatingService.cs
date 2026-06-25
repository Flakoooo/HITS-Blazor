using HITSBlazor.Components.Modals.RightSideModals.IdeaModal;
using HITSBlazor.Models.Common.Responses;
using HITSBlazor.Models.Ideas.Entities;
using HITSBlazor.Models.Ideas.Enums;
using HITSBlazor.Models.Users.Entities;
using HITSBlazor.Services.Ideas;
using HITSBlazor.Utils.Mocks.Ideas;

namespace HITSBlazor.Services.Ratings
{
    public class RatingService(
        IIdeasService ideasService,
        RatingApi ratingApi,
        ILogger<RatingService> logger,
        GlobalNotificationService globalNotificationService
    ) : IRatingService
    {
        private readonly IIdeasService _ideasService = ideasService;
        private readonly RatingApi _ratingApi = ratingApi;
        private readonly ILogger<RatingService> _logger = logger;
        private readonly GlobalNotificationService _globalNotificationService = globalNotificationService;

        public async Task<List<Rating>> GetIdeaRatingsAsync(Guid ideaId)
        {
            //var result
            //if (result.IsSuccess && result.Response is not null)
            //    return result.Response;

            //if (!string.IsNullOrWhiteSpace(result.Message))
            //{
            //    _globalNotificationService.ShowError(result.Message);
            //    if (_logger.IsEnabled(LogLevel.Warning))
            //        _logger.LogWarning("Get users failed: {Error}", result.Message);
            //}

            return [];
        }

        public async Task<bool> SendRatingAsync(RatingRequest request, bool isConfirmed, List<Rating>? ideasRatings)
        {
            if (isConfirmed && ideasRatings is null)
            {
                string errorText = "При подстверждении рейтинга необходимо также указать значение \"ideasRatings\"";
                throw new ArgumentNullException(errorText);
            }


            if (isConfirmed)
            {
                if (!MockRatings.UpdateOrConfirmRating(request, isConfirmed))
                {
                    _globalNotificationService.ShowError("Не удалось подтвердить рейтинг");
                    return false;
                }

                var rating = ideasRatings!.FirstOrDefault(r => r.Id == request.Id);
                if (rating is not null)
                {
                    rating.MarketValue = request.MarketValue;
                    rating.Originality = request.Originality;
                    rating.TechnicalRealizability = request.TechnicalRealizability;
                    rating.Suitability = request.Suitability;
                    rating.Budget = request.Budget;
                    rating.IsConfirmed = true;

                    if (ideasRatings!.Count == ideasRatings!.Count(r => r.IsConfirmed))
                        _ideasService.IdeasStatusHasUpdatedEvent(rating.IdeaId, IdeaStatusType.Confirmed);
                }

                _globalNotificationService.ShowSuccess("Рейтинг успешно подтвержден");
            }
            else
            {
                if (!MockRatings.UpdateOrConfirmRating(request))
                {
                    _globalNotificationService.ShowError("Не удалось сохранить рейтинг");
                    return false;
                }

                _globalNotificationService.ShowSuccess("Рейтинг успешно сохранен");
            }

            return true;
        }
    }
}
