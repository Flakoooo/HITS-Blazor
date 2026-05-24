using HITSBlazor.Models.Common.Responses;
using HITSBlazor.Models.Ideas.Entities;
using HITSBlazor.Models.Markets.Entities;
using HITSBlazor.Models.Markets.Enums;
using HITSBlazor.Models.Users.Enums;
using HITSBlazor.Services.Auth;
using HITSBlazor.Utils.Mocks.Markets;

namespace HITSBlazor.Services.IdeaMarkets
{
    public class MockIdeaMarketService(IAuthService authService) : IIdeaMarketService
    {
        private readonly IAuthService _authService = authService;

        public async Task<ListDataResponse<IdeaMarket>> GetIdeasMarketAsync(
            int page,
            Guid? marketId,
            bool? favorite,
            string? searchText,
            IdeaMarketStatusType? selectedStatus
        )
        {
            var currentUser = _authService.CurrentUser;
            if (currentUser is null || currentUser.Role is null) 
                return new ListDataResponse<IdeaMarket>(0, []);

            return MockIdeaMarkets.GetIdeaMarketsByQueryParams(
                page,
                currentUser.Id,
                (RoleType)currentUser.Role,
                marketId: marketId,
                favorite: favorite,
                searchText: searchText,
                selectedStatus: selectedStatus
            );
        }

        public async Task<IdeaMarket?> GetIdeaMarketAsync(Guid guid) 
            => MockIdeaMarkets.GetIdeaMarketById(guid);

        public async Task<bool> SendIdeasOnMarket(ICollection<Idea> ideas, Market market)
            => MockIdeaMarkets.SendIdeasOnMarket(ideas, market) > 0;

        public async Task<bool> SetIdeaFavorite(Guid userId, IdeaMarket ideaMarket)
            => MockIdeaMarkets.SetIdeaFavorite(userId, ideaMarket.Id);

        public async Task<bool> UnsetIdeaFromFavorite(Guid userId, IdeaMarket ideaMarket)
            => MockIdeaMarkets.UnsetIdeaFromFavorite(userId, ideaMarket.Id);
    }
}
