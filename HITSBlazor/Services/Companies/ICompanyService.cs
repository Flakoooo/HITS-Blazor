using HITSBlazor.Models.Common.Entities;

namespace HITSBlazor.Services.Companies
{
    public interface ICompanyService
    {
        Task<List<Company>> GetCompaniesAsync(string? searchText = null);
    }
}
