using HITSBlazor.Models.Common.Entities;
using HITSBlazor.Models.Markets.Entities;
using HITSBlazor.Models.Markets.Enums;
using HITSBlazor.Models.Users.Entities;
using HITSBlazor.Services.Auth;
using HITSBlazor.Utils.Mocks.Common;
using HITSBlazor.Utils.Mocks.Markets;

namespace HITSBlazor.Services.Markets
{
    public class MockMarketService(
        IAuthService authService,
        GlobalNotificationService globalNotificationService
    ) : IMarketService
    {
        private readonly IAuthService _authService = authService;
        private readonly GlobalNotificationService _globalNotificationService = globalNotificationService;

        public event Func<Task>? OnMarketsStateChanged;
        public event Action? OnMarketsStateUpdated;

        private List<Market> _cachedMarkets = [];
        private DateTime _lastRefreshTime;
        private readonly TimeSpan _cacheLifetime = TimeSpan.FromMinutes(5);

        private async Task RefreshCacheAsync()
        {
            _cachedMarkets = MockMarkets.GetActiveMarkets();
            _lastRefreshTime = DateTime.UtcNow;
        }

        public async Task<List<Market>> GetMarketssAsync(
            string? searchText, 
            HashSet<MarketStatus>? selectedStatuses, 
            string? orderBy, 
            bool? byDescending
        )
        {
            if (_cachedMarkets.Count == 0 || DateTime.UtcNow - _lastRefreshTime > _cacheLifetime)
                await RefreshCacheAsync();

            var query = _cachedMarkets.AsEnumerable();

            if (selectedStatuses?.Count > 0)
                query = query.Where(m => selectedStatuses.Contains(m.Status));

            if (!string.IsNullOrWhiteSpace(searchText))
                query = query.Where(m => m.Name.Contains(searchText, StringComparison.CurrentCultureIgnoreCase));

            if (!string.IsNullOrWhiteSpace(orderBy) && byDescending.HasValue)
            {
                query = (orderBy, byDescending.Value) switch
                {
                    (nameof(Market.StartDate), true) => query.OrderByDescending(m => m.StartDate),
                    (nameof(Market.StartDate), false) => query.OrderBy(m => m.StartDate),
                    (nameof(Market.FinishDate), true) => query.OrderByDescending(m => m.FinishDate),
                    (nameof(Market.FinishDate), false) => query.OrderBy(m => m.FinishDate),
                    _ => query
                };
            }

            return [.. query];
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
            _cachedMarkets.Clear();
            OnMarketsStateChanged?.Invoke();
            return true;
        }

        
        public async Task<bool> UpdateMarketAsync(
            Guid marketId, string name, DateTime startDate, DateTime finishDate, MarketStatus status
        )
        {
            var market = MockMarkets.UpdateMarket(marketId, name, startDate, finishDate, status);
            if (!market)
            {
                _globalNotificationService.ShowError("Не удалось обновить биржу");
                return false;
            }

            var marketForUpdate = _cachedMarkets.FirstOrDefault(m => m.Id == marketId);
            if (marketForUpdate is not null)
            {
                marketForUpdate.Name = name;
                marketForUpdate.StartDate = startDate;
                marketForUpdate.FinishDate = finishDate;
                marketForUpdate.Status = status;
            }

            _globalNotificationService.ShowSuccess("Биржа успешно обновлена");
            OnMarketsStateUpdated?.Invoke();
            return true;
        }

        public async Task UpdateMarketStatusAsync(Guid marketId, MarketStatus status)
        {

        }

        public async Task DeleteMarketAsync(Market market)
        {
            if (!MockMarkets.DeleteMarket(market))
            {
                _globalNotificationService.ShowError("Не удалось удалить команду");
                return;
            }

            _cachedMarkets.Remove(market);
            OnMarketsStateChanged?.Invoke();
            _globalNotificationService.ShowSuccess("Биржа успешно удалена");
            return;
        }
    }
}
