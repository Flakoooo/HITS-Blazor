using HITSBlazor.Models.Common.Responses;
using HITSBlazor.Models.Ideas.Entities;
using HITSBlazor.Models.Markets.Entities;
using HITSBlazor.Models.Markets.Enums;
using HITSBlazor.Models.Teams.Entities;
using HITSBlazor.Models.Users.Entities;
using HITSBlazor.Utils;
using System.Net.Http.Json;

namespace HITSBlazor.Services.IdeaMarkets
{
    public class IdeaMarketApi(
        IHttpClientFactory httpClientFactory, 
        ILogger<IdeaMarketApi> logger
    ) : BaseApiService(httpClientFactory.CreateClient(Settings.HttpClientName), logger)
    {
        private readonly string _ideaMarketPath = "/api/idea_market";

        private const string GET_IDEA_MARKETS_OPERATION = "GetIdeaMarkets";
        private const string GET_IDEA_MARKET_OPERATION = "GetIdeaMarket";
        private const string CREATE_IDEA_MARKETS_OPERATION = "CreateIdeaMarkets";
        private const string SET_IDEA_MARKET_FAVORITE_OPERATION = "SetIdeaFavorite";
        private const string UNSET_IDEA_MARKET_FAVORITE_OPERATION = "UnsetIdeaFavorite";

        public async Task<ServiceResponse<ListDataResponse<IdeaMarket>>> GetIdeaMarketsAsync(
            int page,
            int pageSize = 20,
            Guid? marketId = null,
            bool? favorite = null,
            string? searchText = null,
            IdeaMarketStatusType? selectedStatus = null
        )
        {
            string path = $"{_ideaMarketPath}{AddQuery("page", page, false)}{AddQuery("page_size", pageSize)}";

            if (!string.IsNullOrWhiteSpace(searchText))
                path += AddQuery("search_text", searchText);

            if (marketId.HasValue)
                path += AddQuery("market_id", marketId.Value);

            if (favorite.HasValue)
                path += AddQuery("favorite", favorite.Value);

            if (selectedStatus.HasValue)
                path += AddQuery("selected_status", selectedStatus.Value);

            return await ExecuteApiCallAsync(
                apiCall: () => _httpClient.GetAsync(path),
                successHandler: async response =>
                {
                    var marketIdeas = await response.Content.ReadFromJsonAsync<ListDataResponse<IdeaMarket>>(Settings.BaseJsonOptions);
                    if (marketIdeas is null)
                    {
                        LogFail(GET_IDEA_MARKETS_OPERATION, response.StatusCode, "Error when parse market ideas");

                        return ServiceResponse<ListDataResponse<IdeaMarket>>.Failure("Не удалось получить идеи биржи", response.StatusCode);
                    }

                    return ServiceResponse<ListDataResponse<IdeaMarket>>.Success(marketIdeas);
                },
                operationName: GET_IDEA_MARKETS_OPERATION
            );
        }

        public async Task<ServiceResponse<IdeaMarket?>> GetIdeaMarketAsync(Guid ideaMarketId)
        {
            string path = $"{_ideaMarketPath}/{ideaMarketId}";

            return await ExecuteApiCallAsync(
                apiCall: () => _httpClient.GetAsync(path),
                successHandler: async response =>
                {
                    var marketIdea = await response.Content.ReadFromJsonAsync<IdeaMarket>(Settings.BaseJsonOptions);
                    if (marketIdea is null)
                    {
                        LogFail(GET_IDEA_MARKET_OPERATION, response.StatusCode, "Error when parse market idea");

                        return ServiceResponse<IdeaMarket?>.Failure("Не удалось получить идею биржи", response.StatusCode);
                    }

                    return ServiceResponse<IdeaMarket?>.Success(marketIdea);
                },
                operationName: GET_IDEA_MARKET_OPERATION
            );
        }

        public async Task<ServiceResponse<string>> CreateIdeaMarketsAsync(Guid marketId, IEnumerable<Idea> ideas)
        {
            string path = $"{_ideaMarketPath}/send/{marketId}";
            var content = SerializeData(ideas);

            return await ExecuteApiCallAsync(
                apiCall: () => _httpClient.PostAsync(path, content),
                successHandler: async response =>
                {
                    return ServiceResponse<string>.Success("Выбранные идеи отправлены на биржу");
                },
                operationName: CREATE_IDEA_MARKETS_OPERATION
            );
        }

        public async Task<ServiceResponse<string>> SetIdeaMarketFavoriteAsync(Guid ideaMarketId)
        {
            string path = $"{_ideaMarketPath}/favorite/{ideaMarketId}";

            return await ExecuteApiCallAsync(
                apiCall: () => _httpClient.PutAsync(path, null),
                successHandler: async response =>
                {
                    var message = await response.Content.ReadFromJsonAsync<MessageResponse>(Settings.BaseJsonOptions);
                    if (message is null)
                    {
                        LogFail(SET_IDEA_MARKET_FAVORITE_OPERATION, response.StatusCode, "Error when set market idea favorite");

                        return ServiceResponse<string>.Failure("Не удалось сделать идею избранной", response.StatusCode);
                    }

                    return ServiceResponse<string>.Success(message.Message);
                },
                operationName: SET_IDEA_MARKET_FAVORITE_OPERATION
            );
        }

        public async Task<ServiceResponse<string>> UnsetIdeaMarketFavoriteAsync(Guid ideaMarketId)
        {
            string path = $"{_ideaMarketPath}/favorite/{ideaMarketId}";

            return await ExecuteApiCallAsync(
                apiCall: () => _httpClient.DeleteAsync(path),
                successHandler: async response =>
                {
                    var message = await response.Content.ReadFromJsonAsync<MessageResponse>(Settings.BaseJsonOptions);
                    if (message is null)
                    {
                        LogFail(UNSET_IDEA_MARKET_FAVORITE_OPERATION, response.StatusCode, "Error when unset market idea favorite");

                        return ServiceResponse<string>.Failure("Не удалось убрать идею из избранных", response.StatusCode);
                    }

                    return ServiceResponse<string>.Success(message.Message);
                },
                operationName: UNSET_IDEA_MARKET_FAVORITE_OPERATION
            );
        }
    }
}
