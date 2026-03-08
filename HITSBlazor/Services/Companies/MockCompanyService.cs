using HITSBlazor.Models.Common.Entities;
using HITSBlazor.Utils.Mocks.Common;

namespace HITSBlazor.Services.Companies
{
    public class MockCompanyService : ICompanyService
    {
        private List<Company> _cachedCompanies = [];
        private DateTime _lastRefreshTime;
        private readonly TimeSpan _cacheLifetime = TimeSpan.FromMinutes(5);

        private async Task RefreshCacheAsync()
        {
            _cachedCompanies = MockCompanies.GetAllCompanies();
            _lastRefreshTime = DateTime.UtcNow;
        }

        public async Task<List<Company>> GetCompaniesAsync(string? searchText)
        {
            if (_cachedCompanies.Count == 0 || DateTime.UtcNow - _lastRefreshTime > _cacheLifetime)
                await RefreshCacheAsync();

            var query = _cachedCompanies.AsEnumerable();

            if (!string.IsNullOrWhiteSpace(searchText))
                query = query.Where(c => c.Name.Contains(searchText, StringComparison.CurrentCultureIgnoreCase));

            return [.. query];
        }

        //TODO: Реализовать создание компании как создание навыка
    }
}
