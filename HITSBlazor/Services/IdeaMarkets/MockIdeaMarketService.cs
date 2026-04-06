using HITSBlazor.Models.Markets.Entities;
using HITSBlazor.Models.Markets.Enums;
using HITSBlazor.Models.Teams.Entities;
using HITSBlazor.Services.Auth;
using HITSBlazor.Utils.Mocks.Markets;
using HITSBlazor.Utils.Mocks.Teams;
using HITSBlazor.Utils.Models;

namespace HITSBlazor.Services.IdeaMarkets
{
    //TODO: Сделать так чтобы при роли инициатор выдало только идеи инициатора
    public class MockIdeaMarketService(IAuthService authService) : IIdeaMarketService
    {
        private readonly IAuthService _authService = authService;

        public event Func<Task>? OnIdeasMarketStateChanged;
        public event Action? OnIdeasMarketStateUpdated;

        private Dictionary<Guid, CacheEntry<List<IdeaMarket>>> _cache = [];
        private readonly TimeSpan _cacheLifetime = TimeSpan.FromMinutes(5);

        public async Task<List<IdeaMarket>> GetIdeasMarketAsync(
            Guid marketId,
            bool? favorite,
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

            if (favorite.HasValue)
                query = query.Where(im => im.IsFavorite == favorite.Value);

            if (selectedStatus.HasValue)
                query = query.Where(im => im.Status == selectedStatus);

            if (!string.IsNullOrWhiteSpace(searchText))
                query = query.Where(im => im.Name.Contains(searchText, StringComparison.CurrentCultureIgnoreCase));

            return [.. query];
        }

        public async Task<IdeaMarket?> GetIdeaMarketAsync(Guid guid) 
            => MockIdeaMarkets.GetIdeaMarketById(guid);

        public async Task<List<RequestTeamToIdea>> GetRequestsTeamToIdeaAsync(Guid ideaMarketId)
            => MockRequestTeamToIdeas.GetRequestsByIdeaMarketId(ideaMarketId);
    }
}
