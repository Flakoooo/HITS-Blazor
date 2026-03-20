using HITSBlazor.Models.Markets.Entities;
using HITSBlazor.Models.Markets.Enums;

namespace HITSBlazor.Services.IdeaMarkets
{
    public interface IIdeaMarketService
    {
        Task<List<IdeaMarket>> GetIdeasMarketAsync(
            Guid marketId,
            string? searchText = null,
            IdeaMarketStatusType? selectedStatus = null
        );
    }
}
