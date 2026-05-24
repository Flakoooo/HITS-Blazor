using HITSBlazor.Models.Common.Responses;
using HITSBlazor.Models.Ideas.Entities;
using HITSBlazor.Models.Markets.Entities;
using HITSBlazor.Models.Markets.Enums;

namespace HITSBlazor.Services.IdeaMarkets
{
    public interface IIdeaMarketService
    {
        Task<ListDataResponse<IdeaMarket>> GetIdeasMarketAsync(
            int page,
            Guid? marketId = null,
            bool? favorite = null,
            string? searchText = null,
            IdeaMarketStatusType? selectedStatus = null
        );

        Task<IdeaMarket?> GetIdeaMarketAsync(Guid guid);

        Task<bool> SendIdeasOnMarket(ICollection<Idea> ideas, Market market);

        Task<bool> SetIdeaFavorite(Guid userId, IdeaMarket ideaMarket);
        Task<bool> UnsetIdeaFromFavorite(Guid userId, IdeaMarket ideaMarket);
    }
}
