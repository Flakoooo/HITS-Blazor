using HITSBlazor.Models.Markets.Entities;
using HITSBlazor.Utils.Mocks.Markets;

namespace HITSBlazor.Services.Markets
{
    public class MockMarketService : IMarketService
    {
        public async Task<List<Market>> GetActiveMarketsAsync() => MockMarkets.GetActiveMarkets();
    }
}
