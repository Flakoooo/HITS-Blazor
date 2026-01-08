using HITSBlazor.Models.Markets.Entities;

namespace HITSBlazor.Services.Markets
{
    public interface IMarketService
    {
        Task<List<Market>> GetActiveMarketsAsync();
    }
}
