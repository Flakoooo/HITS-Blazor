using HITSBlazor.Models.Ideas.Entities;
using HITSBlazor.Models.Markets.Entities;
using HITSBlazor.Models.Markets.Enums;
using HITSBlazor.Utils.Mocks.Markets;
using HITSBlazor.Utils.Models;

namespace HITSBlazor.Services.IdeaMarkets
{
    public class MockIdeaMarketService : IIdeaMarketService
    {
        event Func<Task>? OnIdeasMarketStateChanged;
        event Action? OnIdeasMarketStateUpdated;

        private Dictionary<Guid, CacheEntry<List<IdeaMarket>>> _cache = [];
        private readonly TimeSpan _cacheLifetime = TimeSpan.FromMinutes(5);

        public async Task<List<IdeaMarket>> GetIdeasMarketAsync(
            Guid marketId,
            string? searchText,
            IdeaMarketStatusType? selectedStatus
        )
        {
            List<IdeaMarket> ideaMarkets;

            if (_cache.TryGetValue(marketId, out var cache) && !cache.IsExpired(_cacheLifetime))
            {
                ideaMarkets = cache.Data;
            }
            else
            {
                ideaMarkets = MockIdeaMarkets.GetIdeaMarketsByMarketId(marketId);
                _cache[marketId] = new CacheEntry<List<IdeaMarket>>(ideaMarkets);
            }

            var query = ideaMarkets.AsEnumerable();

            if (selectedStatus.HasValue)
                query = query.Where(im => im.Status == selectedStatus);

            if (!string.IsNullOrWhiteSpace(searchText))
                query = query.Where(im => im.Name.Contains(searchText, StringComparison.CurrentCultureIgnoreCase));

            return [.. query];
        }
    }
}
