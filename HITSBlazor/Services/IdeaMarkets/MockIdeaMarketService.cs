using HITSBlazor.Models.Markets.Entities;
using HITSBlazor.Models.Markets.Enums;
using HITSBlazor.Models.Teams.Entities;
using HITSBlazor.Models.Users.Enums;
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

        //private Dictionary<Guid, CacheEntry<List<IdeaMarket>>> _cache = [];
        private Dictionary<Guid, Dictionary<bool, CacheEntry<List<IdeaMarket>>>> _cache = [];
        private readonly TimeSpan _cacheLifetime = TimeSpan.FromMinutes(5);

        public async Task<List<IdeaMarket>> GetIdeasMarketAsync(
            Guid marketId,
            bool? favorite,
            string? searchText,
            IdeaMarketStatusType? selectedStatus
        )
        {
            List<IdeaMarket> ideaMarkets = [];

            if (_authService.CurrentUser is not null)
            {
                if (_cache.TryGetValue(marketId, out var marketCache)
                    && marketCache.TryGetValue(_authService.CurrentUser.Role is RoleType.Initiator, out var cache)
                    && !cache.IsExpired(_cacheLifetime)
                )
                {
                    ideaMarkets = cache.Data;
                }
                else
                {
                    var isInitiator = _authService.CurrentUser.Role is RoleType.Initiator;
                    if (isInitiator)
                        ideaMarkets = MockIdeaMarkets.GetIdeaMarketsByMarketIdAndInitiatorId(marketId, _authService.CurrentUser.Id);
                    else
                        ideaMarkets = MockIdeaMarkets.GetIdeaMarketsByMarketId(marketId, _authService.CurrentUser.Id);

                    _cache[marketId] = new Dictionary<bool, CacheEntry<List<IdeaMarket>>>
                    {
                        [isInitiator] = new CacheEntry<List<IdeaMarket>>(ideaMarkets)
                    };
                }
            }

            var query = ideaMarkets.AsEnumerable();

            if (!query.Any()) return [];

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

        public async Task<List<RequestTeamToIdea>> GetRequestsTeamToIdeaAsync(Guid ideaMarketId, string? searchText)
        {
            var requests = MockRequestTeamToIdeas.GetRequestsByIdeaMarketId(ideaMarketId);

            var query = requests.AsEnumerable();

            if (!string.IsNullOrWhiteSpace(searchText))
                query = query.Where(r => r.Name.Contains(searchText, StringComparison.CurrentCultureIgnoreCase));

            return [.. query];
        }

        public async Task<List<InvitationTeamToIdea>> GetInvitationTeamsToIdeaAsync(Guid ideaId, string? searchText)
        {
            var invitations = MockInvitationTeamToIdeas.GetInvitationTeamsToIdea(ideaId);

            var query = invitations.AsEnumerable();

            if (!string.IsNullOrWhiteSpace(searchText))
                query = query.Where(r => r.TeamName.Contains(searchText, StringComparison.CurrentCultureIgnoreCase));

            return [.. query];
        }

        public async Task<bool> SetIdeaFavorite(Guid userId, IdeaMarket ideaMarket)
        {
            var result = MockIdeaMarkets.SetIdeaFavorite(userId, ideaMarket.Id);
            if (result && _authService.CurrentUser is not null && _cache.TryGetValue(ideaMarket.MarketId, out var marketCache))
                foreach (var cache in marketCache)
                    cache.Value.Data.FirstOrDefault(i => i.Id == ideaMarket.Id)?.IsFavorite = true;

            return result;
        }

        public async Task<bool> UnsetIdeaFromFavorite(Guid userId, IdeaMarket ideaMarket)
        {
            var result = MockIdeaMarkets.UnsetIdeaFromFavorite(userId, ideaMarket.Id);
            if (result && _authService.CurrentUser is not null && _cache.TryGetValue(ideaMarket.MarketId, out var marketCache))
                foreach (var cache in marketCache)
                    cache.Value.Data.FirstOrDefault(i => i.Id == ideaMarket.Id)?.IsFavorite = false;

            return result;
        }
    }
}
