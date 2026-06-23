using HITSBlazor.Models.Common.Entities;
using HITSBlazor.Models.Common.Responses;
using HITSBlazor.Models.Users.Entities;

namespace HITSBlazor.Services.Companies
{
    public class CompanyService(
        CompanyApi companyApi,
        ILogger<CompanyService> logger,
        GlobalNotificationService globalNotificationService
    ) : ICompanyService
    {
        private readonly CompanyApi _companyApi = companyApi;
        private readonly ILogger<CompanyService> _logger = logger;
        private readonly GlobalNotificationService _globalNotificationService = globalNotificationService;

        public event Action<Company>? OnCompanyHasCreated;
        public event Action<Company>? OnCompanyHasUpdated;
        public event Action<Company>? OnCompanyHasDeleted;

        public async Task<ListDataResponse<Company>> GetCompaniesAsync(
            int page, string? searchText
        )
        {
            var result = await _companyApi.GetCompaniesAsync(
                page, searchText: searchText
            );

            if (result.IsSuccess && result.Response is not null)
                return result.Response;

            if (!string.IsNullOrWhiteSpace(result.Message))
            {
                _globalNotificationService.ShowError(result.Message);
                if (_logger.IsEnabled(LogLevel.Warning))
                    _logger.LogWarning("Get companies failed: {Error}", result.Message);
            }

            return new ListDataResponse<Company>(0, []);
        }

        public async Task<Company?> GetCompanyByIdAsync(Guid companyId)
        {
            var result = await _companyApi.GetCompanyAsync(companyId);

            if (!string.IsNullOrWhiteSpace(result.Message))
            {
                _globalNotificationService.ShowError(result.Message);
                if (_logger.IsEnabled(LogLevel.Warning))
                    _logger.LogWarning("Get company failed: {Error}", result.Message);
            }

            return result.Response;
        }
        public async Task<ListDataResponse<User>> GetCompanyMembersAsync(
            Guid companyId, 
            int page, 
            string? searchText
        )
        {
            var result = await _companyApi.GetCompanyMembersAsync(
                companyId, page, searchText: searchText
            );

            if (result.IsSuccess && result.Response is not null)
                return result.Response;

            if (!string.IsNullOrWhiteSpace(result.Message))
            {
                _globalNotificationService.ShowError(result.Message);
                if (_logger.IsEnabled(LogLevel.Warning))
                    _logger.LogWarning("Get company members failed: {Error}", result.Message);
            }

            return new ListDataResponse<User>(0, []);
        }

        public async Task<bool> CreateCompanyAsync(string name, User owner, IEnumerable<User> members)
        {
            var result = await _companyApi.CreateCompanyAsync(
                name, owner.Id, members.Select(m => m.Id)
            );

            if (result.IsSuccess && result.Response is not null)
            {
                OnCompanyHasCreated?.Invoke(result.Response);
                _globalNotificationService.ShowSuccess("Компания успешно создана!");
                return true;
            }

            if (!string.IsNullOrWhiteSpace(result.Message))
            {
                _globalNotificationService.ShowError(result.Message);
                if (_logger.IsEnabled(LogLevel.Warning))
                    _logger.LogWarning("Create company failed: {Error}", result.Message);
            }

            return false;
        }

        public async Task<bool> UpdateCompanyAsync(
            Guid companyId, 
            string? name, 
            User? owner,
            IEnumerable<Guid>? newMembersIds,
            IEnumerable<Guid>? removeMembersIds
        )
        {
            var result = await _companyApi.UpdateCompanyAsync(
                companyId, name, owner?.Id, newMembersIds, removeMembersIds
            );

            if (result.IsSuccess && result.Response is not null)
            {
                OnCompanyHasUpdated?.Invoke(result.Response);
                _globalNotificationService.ShowSuccess("Компания успешно обновлена!");
                return true;
            }

            if (!string.IsNullOrWhiteSpace(result.Message))
            {
                _globalNotificationService.ShowError(result.Message);
                if (_logger.IsEnabled(LogLevel.Warning))
                    _logger.LogWarning("Update company failed: {Error}", result.Message);
            }

            return false;
        }

        public async Task<bool> DeleteCompanyAsync(Company company)
        {
            var result = await _companyApi.DeleteCompanyAsync(company.Id);

            if (result.IsSuccess && result.Response is not null)
            {
                OnCompanyHasDeleted?.Invoke(company);
                _globalNotificationService.ShowSuccess("Компания успешно удалена!");
                return true;
            }

            if (!string.IsNullOrWhiteSpace(result.Message))
            {
                _globalNotificationService.ShowError(result.Message);
                if (_logger.IsEnabled(LogLevel.Warning))
                    _logger.LogWarning("Delete company failed: {Error}", result.Message);
            }

            return false;
        }
    }
}
