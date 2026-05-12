using HITSBlazor.Models.Common.Responses;
using HITSBlazor.Models.Markets.Entities;
using HITSBlazor.Models.Markets.Enums;

namespace HITSBlazor.Services.Markets
{
    public interface IMarketService
    {
        event Action<Market>? OnMarketsHasCreated;
        event Action<Market>? OnMarketHasUpdated;
        event Action<Market>? OnMarketStatusHasUpdated;
        event Action<Market>? OnMarketHasDeleted;

        Task<ListDataResponse<Market>> GetMarketsAsync(
            int page,
            string? searchText = null,
            IEnumerable<MarketStatus>? selectedStatuses = null,
            string? orderBy = null,
            bool? byDescending = null
        );
        Task<List<Market>> GetAllActiveMarketsAsync();
        Task<Market?> GetMarketByIdAsync(Guid marketId);
        Task<bool> CreateNewMarketAsync(string name, DateTime startDate, DateTime finishDate);
        Task<bool> UpdateMarketAsync(
            Guid marketId, string name, DateTime startDate, DateTime finishDate, MarketStatus status
        );
        Task UpdateMarketStatusAsync(Market market, MarketStatus status);
        Task DeleteMarketAsync(Market market);
    }
}
