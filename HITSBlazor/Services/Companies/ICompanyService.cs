using HITSBlazor.Models.Common.Entities;
using HITSBlazor.Models.Users.Entities;

namespace HITSBlazor.Services.Companies
{
    public interface ICompanyService
    {
        event Action? OnCompaniesStateChanged;

        Task<List<Company>> GetCompaniesAsync(string? searchText = null);
        Task<Company?> GetCompanyByIdAsync(Guid companyId);
        Task<Company?> GetCompanyByNameAsync(string name);
        Task<bool> CreateCompany(string name, User owner, List<User> members);
        Task<bool> UpdateCompany(Guid companyId, string name, User owner, List<User> members);
        Task<bool> DeleteCompanyAsync(Company company);
    }
}
