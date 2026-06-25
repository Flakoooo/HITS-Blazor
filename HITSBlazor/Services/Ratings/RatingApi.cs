using HITSBlazor.Components.Modals.RightSideModals.IdeaModal;
using HITSBlazor.Models.Common.Responses;
using HITSBlazor.Models.Ideas.Entities;
using HITSBlazor.Utils;
using System.Net.Http.Json;

namespace HITSBlazor.Services.Ratings
{
    public class RatingApi(
        IHttpClientFactory httpClientFactory,
        ILogger<RatingApi> logger
    ) : BaseApiService(httpClientFactory.CreateClient(Settings.HttpClientName), logger)
    {
        private readonly string _ratingPath = "/api/rating";

        private const string GET_RATINGS_OPERATION = "GetRatings";
        private const string SAVE_RATING_OPERATION = "SaveRating";

        public async Task<ServiceResponse<List<Rating>>> GetRatingsAsync(Guid ideaId)
        {
            string path = $"{_ratingPath}/{ideaId}";

            return await ExecuteApiCallAsync(
                apiCall: () => _httpClient.GetAsync(path),
                successHandler: async response =>
                {
                    var ratings = await response.Content.ReadFromJsonAsync<List<Rating>>(Settings.BaseJsonOptions);
                    if (ratings is null)
                    {
                        LogFail(GET_RATINGS_OPERATION, response.StatusCode, "Error when parse ratings");

                        return ServiceResponse<List<Rating>>.Failure("Не удалось удалить оценки идеи", response.StatusCode);
                    }

                    return ServiceResponse<List<Rating>>.Success(ratings);
                },
                operationName: GET_RATINGS_OPERATION
            );
        }

        public async Task<ServiceResponse<string>> SaveRatingsAsync(RatingRequest request)
        {
            string path = $"{_ratingPath}/save";
            var content = SerializeData(request);

            return await ExecuteApiCallAsync(
                apiCall: () => _httpClient.PutAsync(path, content),
                successHandler: async response =>
                {
                    var messsage = await response.Content.ReadFromJsonAsync<MessageResponse>(Settings.BaseJsonOptions);
                    if (messsage is null)
                    {
                        LogFail(GET_RATINGS_OPERATION, response.StatusCode, "Error when save rating");

                        return ServiceResponse<string>.Failure("Не удалось удалить сохранить оценку", response.StatusCode);
                    }

                    return ServiceResponse<string>.Success(messsage.Message);
                },
                operationName: SAVE_RATING_OPERATION
            );
        }
    }
}
