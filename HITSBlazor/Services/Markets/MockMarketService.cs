using HITSBlazor.Models.Common.Responses;
using HITSBlazor.Models.Markets.Entities;
using HITSBlazor.Models.Markets.Enums;
using HITSBlazor.Utils.Mocks.Markets;

namespace HITSBlazor.Services.Markets
{
    public class MockMarketService(
        GlobalNotificationService globalNotificationService
    ) : IMarketService
    {
        private readonly GlobalNotificationService _globalNotificationService = globalNotificationService;

        public event Action<Market>? OnMarketsHasCreated;
        public event Action<Market>? OnMarketHasUpdated;
        public event Action<Guid, MarketStatus>? OnMarketStatusHasUpdated;
        public event Action<Market>? OnMarketHasDeleted;

        public async Task<ListDataResponse<Market>> GetMarketsAsync(
            int page,
            string? searchText, 
            IEnumerable<MarketStatus>? selectedStatuses, 
            string? orderBy, 
            bool? byDescending
        ) => MockMarkets.GetMarketsByQueryParams(
            page,
            searchText: searchText,
            selectedStatuses: selectedStatuses?.ToHashSet(),
            orderBy: orderBy,
            byDescending: byDescending
        );

        public async Task<Market?> GetMarketByIdAsync(Guid marketId)
        {
            var market = MockMarkets.GetMarketById(marketId);
            if (market is null)
            {
                _globalNotificationService.ShowError("Не удалось получить биржу");
                return market;
            }

            return market;
        }

        public async Task<bool> CreateNewMarketAsync(string name, DateTime startDate, DateTime finishDate)
        {
            var market = MockMarkets.CreateMarket(name, startDate, finishDate);
            if (market is null)
            {
                _globalNotificationService.ShowError("Не удалось создать биржу");
                return false;
            }

            _globalNotificationService.ShowSuccess("Биржа успешно создана");
            OnMarketsHasCreated?.Invoke(market);
            return true;
        }

        
        public async Task<bool> UpdateMarketAsync(
            Guid marketId, string name, DateTime startDate, DateTime finishDate, MarketStatus status
        )
        {
            var market = MockMarkets.UpdateMarket(marketId, name, startDate, finishDate, status);
            if (market is null)
            {
                _globalNotificationService.ShowError("Не удалось обновить биржу");
                return false;
            }

            _globalNotificationService.ShowSuccess("Биржа успешно обновлена");
            OnMarketHasUpdated?.Invoke(market);
            return true;
        }

        public async Task UpdateMarketStatusAsync(Guid marketId, MarketStatus status)
        {
            var updatedmarket = MockMarkets.UpdateMarketStatus(marketId, status);
            if (!updatedmarket)
            {
                string error = status switch
                {
                    MarketStatus.Active => "Не удалось открыть биржу",
                    MarketStatus.Done => "Не удалось закрыть биржу",
                    _ => "Не удалось перевести биржу"
                };

                _globalNotificationService.ShowError(error);
                return;
            }

            string success = status switch
            {
                MarketStatus.Active => "Биржа успешно открыта",
                MarketStatus.Done => "Биржа успешно закрыта",
                _ => "Биржа успешно переведена"
            };

            _globalNotificationService.ShowSuccess(success);
            OnMarketStatusHasUpdated?.Invoke(marketId, status);
            return;
        }

        public async Task DeleteMarketAsync(Market market)
        {
            if (!MockMarkets.DeleteMarket(market))
            {
                _globalNotificationService.ShowError("Не удалось удалить команду");
                return;
            }

            _globalNotificationService.ShowSuccess("Биржа успешно удалена");
            OnMarketHasDeleted?.Invoke(market);
            return;
        }
    }
}
