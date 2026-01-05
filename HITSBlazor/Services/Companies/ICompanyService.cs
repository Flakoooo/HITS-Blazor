using HITSBlazor.Models.Common.Entities;
using HITSBlazor.Utils;

namespace HITSBlazor.Services.Companies
{
    public interface ICompanyService
    {
        Task<ServiceResponse<List<Company>>> GetAllCompanies();
        Task<List<Company>> GetCompaniesByName(string name);
    }
}
