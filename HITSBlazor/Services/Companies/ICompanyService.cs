using HITSBlazor.Models.Common.Entities;
using HITSBlazor.Models.Users.Entities;
using HITSBlazor.Models.Users.Enums;

namespace HITSBlazor.Services.Companies
{
    public interface ICompanyService
    {
        event Action? OnCompaniesStateChanged;

        Task<List<Company>> GetCompaniesAsync(string? searchText = null, RoleType? role = null);
        Task<Company?> GetCompanyByIdAsync(Guid companyId);
        Task<Company?> GetCompanyByNameAsync(string name);
        Task<bool> CreateCompanyAsync(string name, User owner, List<User> members);
        Task<bool> UpdateCompanyAsync(Guid companyId, string name, User owner, List<User> members);
        Task<bool> DeleteCompanyAsync(Company company);
    }
}
