using HITSBlazor.Models.Common.Responses;
using HITSBlazor.Models.Markets.Entities;
using HITSBlazor.Models.Markets.Enums;
using HITSBlazor.Models.Teams.Entities;
using HITSBlazor.Models.Users.Entities;
using HITSBlazor.Services.Users;
using HITSBlazor.Utils.Mocks.Markets;
using System.Xml.Linq;

namespace HITSBlazor.Services.Markets
{
    public class MarketService(
        MarketApi marketApi,
        ILogger<MarketService> logger,
        GlobalNotificationService globalNotificationService
    ) : IMarketService
    {
        private readonly MarketApi _marketApi = marketApi;
        private readonly ILogger<MarketService> _logger = logger;
        private readonly GlobalNotificationService _globalNotificationService = globalNotificationService;

        public event Action<Market>? OnMarketsHasCreated;
        public event Action<Market>? OnMarketHasUpdated;
        public event Action<Market>? OnMarketStatusHasUpdated;
        public event Action<Market>? OnMarketHasDeleted;

        public async Task<ListDataResponse<Market>> GetMarketsAsync(
            int page,
            string? searchText, 
            IEnumerable<MarketStatus>? selectedStatuses, 
            string? orderBy, 
            bool? byDescending
        )
        {
            var result = await _marketApi.GetMarketsAsync(
                page,
                searchText: searchText,
                selectedStatuses: selectedStatuses,
                orderBy: orderBy,
                byDescending: byDescending
            );

            if (result.IsSuccess && result.Response is not null)
                return result.Response;

            if (!string.IsNullOrWhiteSpace(result.Message))
            {
                _globalNotificationService.ShowError(result.Message);
                if (_logger.IsEnabled(LogLevel.Warning))
                    _logger.LogWarning("Get markets failed: {Error}", result.Message);
            }

            return new ListDataResponse<Market>(0, []);
        }

        public async Task<List<Market>> GetAllActiveMarketsAsync()
        {
            var result = await _marketApi.GetActiveMarketsAsync();
            if (result.IsSuccess && result.Response is not null)
                return result.Response.ToList();

            if (!string.IsNullOrWhiteSpace(result.Message))
            {
                _globalNotificationService.ShowError(result.Message);
                if (_logger.IsEnabled(LogLevel.Warning))
                    _logger.LogWarning("Get active markets failed: {Error}", result.Message);
            }

            return [];
        }

        public async Task<Market?> GetMarketByIdAsync(Guid marketId)
        {
            var result = await _marketApi.GetMarketAsync(marketId);

            if (!string.IsNullOrWhiteSpace(result.Message))
            {
                _globalNotificationService.ShowError(result.Message);
                if (_logger.IsEnabled(LogLevel.Warning))
                    _logger.LogWarning("Get market failed: {Error}", result.Message);
            }

            return result.Response;
        }

        public async Task<bool> CreateNewMarketAsync(string name, DateOnly startDate, DateOnly finishDate)
        {
            var result = await _marketApi.CreateMarketAsync(name, startDate, finishDate);
            if (result.IsSuccess && result.Response is not null)
            {
                _globalNotificationService.ShowSuccess("Биржа успешно создана");
                OnMarketsHasCreated?.Invoke(result.Response);
                return true;
            }

            if (!string.IsNullOrWhiteSpace(result.Message))
            {
                _globalNotificationService.ShowError(result.Message);
                if (_logger.IsEnabled(LogLevel.Warning))
                    _logger.LogWarning("Create market failed: {Error}", result.Message);
            }

            return false;
        }

        
        public async Task<bool> UpdateMarketAsync(
            Guid marketId, string name, DateOnly startDate, DateOnly finishDate
        )
        {
            var result = await _marketApi.UpdateMarketAsync(marketId, name, startDate, finishDate);
            if (result.IsSuccess && result.Response is not null)
            {
                _globalNotificationService.ShowSuccess("Биржа успешно изменена");
                OnMarketHasUpdated?.Invoke(result.Response);
                return true;
            }

            if (!string.IsNullOrWhiteSpace(result.Message))
            {
                _globalNotificationService.ShowError(result.Message);
                if (_logger.IsEnabled(LogLevel.Warning))
                    _logger.LogWarning("Update market failed: {Error}", result.Message);
            }

            return false;
        }

        public async Task UpdateMarketStatusAsync(Market market, MarketStatus status)
        {
            var result = await _marketApi.UpdateMarketStatusAsync(market.Id, status);
            if (result.IsSuccess && result.Response is not null)
            {
                string success = status switch
                {
                    MarketStatus.Active => "Биржа успешно открыта",
                    MarketStatus.Done => "Биржа успешно закрыта",
                    _ => "Биржа успешно переведена"
                };

                _globalNotificationService.ShowSuccess(success);
                OnMarketStatusHasUpdated?.Invoke(result.Response);
            }
            else if (!string.IsNullOrWhiteSpace(result.Message))
            {
                string error = status switch
                {
                    MarketStatus.Active => "Не удалось открыть биржу",
                    MarketStatus.Done => "Не удалось закрыть биржу",
                    _ => "Не удалось перевести биржу"
                };
                _globalNotificationService.ShowError(error);
                if (_logger.IsEnabled(LogLevel.Warning))
                    _logger.LogWarning("Update market status failed: {Error}", result.Message);
            }
        }

        public async Task DeleteMarketAsync(Market market)
        {
            var result = await _marketApi.DeleteMarketAsync(market.Id);
            if (result.IsSuccess && result.Response is not null)
            {
                _globalNotificationService.ShowSuccess("Биржа успешно удалена");
                OnMarketHasDeleted?.Invoke(market);
            }
            else if (!string.IsNullOrWhiteSpace(result.Message))
            {
                _globalNotificationService.ShowError(result.Message);
                if (_logger.IsEnabled(LogLevel.Warning))
                    _logger.LogWarning("Update market failed: {Error}", result.Message);
            }
        }
    }
}
