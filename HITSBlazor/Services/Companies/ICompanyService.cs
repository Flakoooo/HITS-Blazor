using HITSBlazor.Models.Common.Entities;
using HITSBlazor.Models.Common.Responses;
using HITSBlazor.Models.Users.Entities;

namespace HITSBlazor.Services.Companies
{
    public interface ICompanyService
    {
        event Action<Company>? OnCompanyHasCreated;
        event Action<Company>? OnCompanyHasUpdated;
        event Action<Company>? OnCompanyHasDeleted;

        Task<ListDataResponse<Company>> GetCompaniesAsync(
            int page, string? searchText = null
        );
        Task<Company?> GetCompanyByIdAsync(Guid companyId);
        Task<Company?> GetCompanyByNameAsync(string name);
        Task<bool> CreateCompanyAsync(string name, User owner, HashSet<User> members);
        Task<bool> UpdateCompanyAsync(Guid companyId, string name, User owner, HashSet<User> members);
        Task<bool> DeleteCompanyAsync(Company company);
    }
}
