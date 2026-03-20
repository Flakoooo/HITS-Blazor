using HITSBlazor.Models.Markets.Entities;
using HITSBlazor.Models.Markets.Enums;

namespace HITSBlazor.Services.Markets
{
    public interface IMarketService
    {
        event Func<Task>? OnMarketsStateChanged;
        event Action? OnMarketsStateUpdated;

        Task<List<Market>> GetMarketsAsync(
            string? searchText = null, 
            HashSet<MarketStatus>? selectedStatuses = null,
            string? orderBy = null,
            bool? byDescending = null
        );
        Task<bool> CreateNewMarketAsync(string name, DateTime startDate, DateTime finishDate);
        Task<bool> UpdateMarketAsync(
            Guid marketId, string name, DateTime startDate, DateTime finishDate, MarketStatus status
        );
        Task UpdateMarketStatusAsync(Guid marketId, MarketStatus status);
        Task DeleteMarketAsync(Market market);
    }
}
