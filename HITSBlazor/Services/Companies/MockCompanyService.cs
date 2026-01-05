using HITSBlazor.Models.Common.Entities;
using HITSBlazor.Utils;
using HITSBlazor.Utils.Mocks.Common;

namespace HITSBlazor.Services.Companies
{
    public class MockCompanyService : ICompanyService
    {
        private List<Company> _companies = [];

        public async Task<ServiceResponse<List<Company>>> GetAllCompanies()
        {
            _companies = MockCompanies.GetAllCompanies();

            return ServiceResponse<List<Company>>.Success(_companies);
        }

        public async Task<List<Company>> GetCompaniesByName(string name)
        {
            if (_companies.Count == 0)
                await GetAllCompanies();

            return [.. _companies.Where(c => c.Name.Contains(name, StringComparison.CurrentCultureIgnoreCase))];
        }
    }
}
