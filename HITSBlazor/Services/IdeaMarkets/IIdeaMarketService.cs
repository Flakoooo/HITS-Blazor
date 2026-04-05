using HITSBlazor.Models.Markets.Entities;
using HITSBlazor.Models.Markets.Enums;

namespace HITSBlazor.Services.IdeaMarkets
{
    public interface IIdeaMarketService
    {
        event Func<Task>? OnIdeasMarketStateChanged;
        event Action? OnIdeasMarketStateUpdated;

        Task<List<IdeaMarket>> GetIdeasMarketAsync(
            Guid marketId,
            bool? favorite = null,
            string? searchText = null,
            IdeaMarketStatusType? selectedStatus = null
        );

        Task<IdeaMarket?> GetIdeaMarketAsync(Guid guid);
    }
}
