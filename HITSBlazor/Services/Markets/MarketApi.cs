using HITSBlazor.Models.Common.Responses;
using HITSBlazor.Models.Markets.Entities;
using HITSBlazor.Models.Markets.Enums;
using HITSBlazor.Models.Users.Entities;
using HITSBlazor.Utils;
using System.Net.Http.Json;

namespace HITSBlazor.Services.Markets
{
    public class MarketApi(
        IHttpClientFactory httpClientFactory, 
        ILogger<MarketApi> logger
    ) : BaseApiService(httpClientFactory.CreateClient(Settings.HttpClientName), logger)
    {
        private readonly string _marketPath = "/api/market";

        private const string GET_MARKETS_OPERATION = "GetMarkets";
        private const string GET_ACTIVE_MARKETS_OPERATION = "GetActiveMarkets";
        private const string GET_MARKET_OPERATION = "GetMarket";
        private const string CREATE_MARKET_OPERATION = "CreateMarket";
        private const string UPDATE_MARKET_OPERATION = "UpdateMarket";
        private const string UPDATE_MARKET_STATUS_OPERATION = "UpdateMarketStatus";
        private const string DELETE_MARKET_OPERATION = "DeleteMarket";

        public async Task<ServiceResponse<ListDataResponse<Market>>> GetMarketsAsync(
            int page,
            int pageSize = 20,
            string? searchText = null,
            IEnumerable<MarketStatus>? selectedStatuses = null,
            string? orderBy = null,
            bool? byDescending = null
        )
        {
            string path = $"{_marketPath}{AddQuery("page", page, false)}{AddQuery("page_size", pageSize)}";

            if (!string.IsNullOrWhiteSpace(searchText))
                path += AddQuery("search_text", searchText);

            if (selectedStatuses is not null && selectedStatuses.Any())
                path += AddQuery("selected_status", selectedStatuses);

            if (!string.IsNullOrWhiteSpace(orderBy))
                path += AddQuery("order_by", orderBy);

            if (byDescending.HasValue)
                path += AddQuery("by_descending", byDescending.Value);

            return await ExecuteApiCallAsync(
                apiCall: () => _httpClient.GetAsync(path),
                successHandler: async response =>
                {
                    var markets = await response.Content.ReadFromJsonAsync<ListDataResponse<Market>>(Settings.BaseJsonOptions);
                    if (markets is null)
                    {
                        LogFail(GET_MARKETS_OPERATION, response.StatusCode, "Error when parse markets");

                        return ServiceResponse<ListDataResponse<Market>>.Failure("Не удалось получить биржи", response.StatusCode);
                    }

                    return ServiceResponse<ListDataResponse<Market>>.Success(markets);
                },
                operationName: GET_MARKETS_OPERATION
            );
        }

        public async Task<ServiceResponse<IEnumerable<Market>>> GetActiveMarketsAsync()
        {
            string path = $"{_marketPath}/active";

            return await ExecuteApiCallAsync(
                apiCall: () => _httpClient.GetAsync(path),
                successHandler: async response =>
                {
                    var markets = await response.Content.ReadFromJsonAsync<IEnumerable<Market>>(Settings.BaseJsonOptions);
                    if (markets is null)
                    {
                        LogFail(GET_ACTIVE_MARKETS_OPERATION, response.StatusCode, "Error when parse markets");

                        return ServiceResponse<IEnumerable<Market>>.Failure("Не удалось получить активные биржи", response.StatusCode);
                    }

                    return ServiceResponse<IEnumerable<Market>>.Success(markets);
                },
                operationName: GET_ACTIVE_MARKETS_OPERATION
            );
        }

        public async Task<ServiceResponse<Market?>> GetMarketAsync(Guid marketId)
        {
            string path = $"{_marketPath}/{marketId}";

            return await ExecuteApiCallAsync(
                apiCall: () => _httpClient.GetAsync(path),
                successHandler: async response =>
                {
                    var markets = await response.Content.ReadFromJsonAsync<Market>(Settings.BaseJsonOptions);
                    if (markets is null)
                    {
                        LogFail(GET_MARKET_OPERATION, response.StatusCode, "Error when parse market");

                        return ServiceResponse<Market?>.Failure("Не удалось получить биржу", response.StatusCode);
                    }

                    return ServiceResponse<Market?>.Success(markets);
                },
                operationName: GET_MARKET_OPERATION
            );
        }

        public async Task<ServiceResponse<Market?>> CreateMarketAsync(string name, DateOnly startDate, DateOnly finishDate)
        {
            var content = SerializeData(new { Name = name, StartDate = startDate, FinishDate = finishDate });

            return await ExecuteApiCallAsync(
                apiCall: () => _httpClient.PostAsync(_marketPath, content),
                successHandler: async response =>
                {
                    var markets = await response.Content.ReadFromJsonAsync<Market>(Settings.BaseJsonOptions);
                    if (markets is null)
                    {
                        LogFail(CREATE_MARKET_OPERATION, response.StatusCode, "Error when create market");

                        return ServiceResponse<Market?>.Failure("Не удалось создать биржу", response.StatusCode);
                    }

                    return ServiceResponse<Market?>.Success(markets);
                },
                operationName: CREATE_MARKET_OPERATION
            );
        }

        public async Task<ServiceResponse<Market?>> UpdateMarketAsync(Guid marketId, string name, DateOnly startDate, DateOnly finishDate)
        {
            var content = SerializeData(new { Id = marketId, Name = name, StartDate = startDate, FinishDate = finishDate });

            return await ExecuteApiCallAsync(
                apiCall: () => _httpClient.PutAsync(_marketPath, content),
                successHandler: async response =>
                {
                    var markets = await response.Content.ReadFromJsonAsync<Market>(Settings.BaseJsonOptions);
                    if (markets is null)
                    {
                        LogFail(UPDATE_MARKET_OPERATION, response.StatusCode, "Error when update market");

                        return ServiceResponse<Market?>.Failure("Не удалось изменить биржу", response.StatusCode);
                    }

                    return ServiceResponse<Market?>.Success(markets);
                },
                operationName: UPDATE_MARKET_OPERATION
            );
        }

        public async Task<ServiceResponse<Market?>> UpdateMarketStatusAsync(Guid marketId, MarketStatus newStatus)
        {
            string path = $"{_marketPath}/status";
            var content = SerializeData(new { Id = marketId, Status = newStatus });

            return await ExecuteApiCallAsync(
                apiCall: () => _httpClient.PutAsync(path, content),
                successHandler: async response =>
                {
                    var markets = await response.Content.ReadFromJsonAsync<Market>(Settings.BaseJsonOptions);
                    if (markets is null)
                    {
                        LogFail(UPDATE_MARKET_STATUS_OPERATION, response.StatusCode, "Error when update market status");

                        return ServiceResponse<Market?>.Failure("Не удалось изменить статус биржи", response.StatusCode);
                    }

                    return ServiceResponse<Market?>.Success(markets);
                },
                operationName: UPDATE_MARKET_STATUS_OPERATION
            );
        }

        public async Task<ServiceResponse<string>> DeleteMarketAsync(Guid marketId)
        {
            string path = $"{_marketPath}/{marketId}";

            return await ExecuteApiCallAsync(
                apiCall: () => _httpClient.DeleteAsync(path),
                successHandler: async response =>
                {
                    var message = await response.Content.ReadFromJsonAsync<MessageResponse>(Settings.BaseJsonOptions);
                    if (message is null)
                    {
                        LogFail(DELETE_MARKET_OPERATION, response.StatusCode, "Error when delete market");

                        return ServiceResponse<string>.Failure("Не удалось удалить биржу", response.StatusCode);
                    }

                    return ServiceResponse<string>.Success(message.Message);
                },
                operationName: DELETE_MARKET_OPERATION
            );
        }
    }
}
