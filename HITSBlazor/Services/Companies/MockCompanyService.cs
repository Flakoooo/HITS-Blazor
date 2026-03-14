using HITSBlazor.Models.Common.Entities;
using HITSBlazor.Models.Quests.Entities;
using HITSBlazor.Models.Users.Entities;
using HITSBlazor.Utils.Mocks.Common;

namespace HITSBlazor.Services.Companies
{
    public class MockCompanyService(GlobalNotificationService globalNotificationService) : ICompanyService
    {
        private readonly GlobalNotificationService _globalNotificationService = globalNotificationService;

        public event Action? OnCompaniesStateChanged;

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

        public async Task<Company?> GetCompanyByIdAsync(Guid companyId)
        {
            var company = MockCompanies.GetCompanyById(companyId);
            if (company is null)
                _globalNotificationService.ShowError("Компания не найдена");

            return company;
        }

        public async Task<Company?> GetCompanyByNameAsync(string name)
        {
            if (_cachedCompanies.Count == 0 || DateTime.UtcNow - _lastRefreshTime > _cacheLifetime)
                await RefreshCacheAsync();

            return _cachedCompanies.FirstOrDefault(c => c.Name.Equals(name, StringComparison.CurrentCultureIgnoreCase));
        }

        public async Task<bool> CreateCompany(string name, User owner, List<User> members)
        {
            var company = MockCompanies.CreateCompany(name, owner.Id, members);
            if (company is null)
            {
                _globalNotificationService.ShowError("Не удалось создать команду");
                return false;
            }

            _globalNotificationService.ShowSuccess("Компания успешно создана");
            _cachedCompanies.Add(company);
            OnCompaniesStateChanged?.Invoke();
            return true;
        }

        public async Task<bool> UpdateCompany(Guid companyId, string name, User owner, List<User> members)
        {
            var company = MockCompanies.UpdateCompany(companyId, name, owner.Id, members);
            if (company is null)
            {
                _globalNotificationService.ShowError("Не удалось обновить команду");
                return false;
            }

            var companyForUpdate = _cachedCompanies.FirstOrDefault(u => u.Id == companyId);
            if (companyForUpdate is not null)
            {
                company.Name = name;
                company.Owner = owner;
                company.Members = [.. members];
            }

            _globalNotificationService.ShowSuccess("Компания успешно обновлена");
            OnCompaniesStateChanged?.Invoke();
            return true;
        }

        public async Task<bool> DeleteCompanyAsync(Company company)
        {
            if (!MockCompanies.DeleteCompany(company))
            {
                _globalNotificationService.ShowError("Не удалось удалить команду");
                return false;
            }

            _cachedCompanies.Remove(company);
            return true;
        }
    }
}
