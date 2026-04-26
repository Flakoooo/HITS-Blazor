using HITSBlazor.Models.Common.Entities;
using HITSBlazor.Models.Common.Responses;
using HITSBlazor.Models.Users.Entities;
using HITSBlazor.Utils.Mocks.Common;

namespace HITSBlazor.Services.Companies
{
    public class MockCompanyService(
        GlobalNotificationService globalNotificationService
    ) : ICompanyService
    {
        private readonly GlobalNotificationService _globalNotificationService = globalNotificationService;

        public event Action<Company>? OnCompanyHasCreated;
        public event Action<Company>? OnCompanyHasUpdated;
        public event Action<Company>? OnCompanyHasDeleted;

        public async Task<ListDataResponse<Company>> GetCompaniesAsync(
            int page, string? searchText
        ) => MockCompanies.GetAllCompaniesByQueryParams(
            page,
            searchText: searchText
        );

        public async Task<Company?> GetCompanyByIdAsync(Guid companyId)
        {
            var company = MockCompanies.GetCompanyById(companyId);
            if (company is null)
                _globalNotificationService.ShowError("Компания не найдена");

            return company;
        }

        //TODO: надо это как то изменить
        public async Task<Company?> GetCompanyByNameAsync(string name)
        {
            return MockCompanies.GetCompanyByName(name);
        }

        public async Task<bool> CreateCompanyAsync(string name, User owner, HashSet<User> members)
        {
            var company = MockCompanies.CreateCompany(name, owner.Id, members.Select(u => u.Id).ToList());
            if (company is null)
            {
                _globalNotificationService.ShowError("Не удалось создать команду");
                return false;
            }

            _globalNotificationService.ShowSuccess("Компания успешно создана");
            OnCompanyHasCreated?.Invoke(company);
            return true;
        }

        public async Task<bool> UpdateCompanyAsync(Guid companyId, string name, User owner, HashSet<User> members)
        {
            var company = MockCompanies.UpdateCompany(companyId, name, owner.Id, members.Select(u => u.Id).ToList());
            if (company is null)
            {
                _globalNotificationService.ShowError("Не удалось обновить команду");
                return false;
            }

            _globalNotificationService.ShowSuccess("Компания успешно обновлена");
            OnCompanyHasUpdated?.Invoke(company);
            return true;
        }

        public async Task<bool> DeleteCompanyAsync(Company company)
        {
            if (!MockCompanies.DeleteCompany(company))
            {
                _globalNotificationService.ShowError("Не удалось удалить команду");
                return false;
            }

            OnCompanyHasDeleted?.Invoke(company);

            return true;
        }
    }
}
