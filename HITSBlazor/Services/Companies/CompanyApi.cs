using ApexCharts;
using HITSBlazor.Models.Common.Entities;
using HITSBlazor.Models.Common.Responses;
using HITSBlazor.Models.Users.Entities;
using HITSBlazor.Utils;
using System.Net.Http.Json;

namespace HITSBlazor.Services.Companies
{
    public class CompanyApi(
        IHttpClientFactory httpClientFactory, 
        ILogger<CompanyApi> logger
    ) : BaseApiService(httpClientFactory.CreateClient(Settings.HttpClientName), logger)
    {
        private readonly string _companyPath = "api/company";

        private const string GET_COMPANIES_OPERATION = "GetCompanies";
        private const string GET_COMPANY_OPERATION = "GetCompany";
        private const string GET_COMPANY_MEMBERS_OPERATION = "GetCompanyMembers";
        private const string CREATE_COMPANY_OPERATION = "CreateCompany";
        private const string UPDATE_COMPANY_OPERATION = "UpdateCompany";
        private const string DELETE_COMPANY_OPERATION = "DeleteCompany";

        public async Task<ServiceResponse<ListDataResponse<Company>>> GetCompaniesAsync(
            int page,
            int pageSize = 20,
            string? searchText = null
        )
        {
            string path = $"{_companyPath}{AddQuery("page", page, false)}{AddQuery("page_size", pageSize)}";

            if (!string.IsNullOrWhiteSpace(searchText))
                path += AddQuery("search_text", searchText);

            return await ExecuteApiCallAsync(
                apiCall: () => _httpClient.GetAsync(path),
                successHandler: async response =>
                {
                    var companies = await response.Content.ReadFromJsonAsync<ListDataResponse<Company>>(Settings.UserJsonOptions);
                    if (companies is null)
                    {
                        LogFail(GET_COMPANIES_OPERATION, response.StatusCode, "Error when parse companies");

                        return ServiceResponse<ListDataResponse<Company>>.Failure("Не удалось получить компании", response.StatusCode);
                    }

                    return ServiceResponse<ListDataResponse<Company>>.Success(companies);
                },
                operationName: GET_COMPANIES_OPERATION
            );
        }

        public async Task<ServiceResponse<Company?>> GetCompanyAsync(Guid companyId)
        {
            string path = $"{_companyPath}/{companyId}";

            return await ExecuteApiCallAsync(
                apiCall: () => _httpClient.GetAsync(path),
                successHandler: async response =>
                {
                    var company = await response.Content.ReadFromJsonAsync<Company>(Settings.UserJsonOptions);
                    if (company is null)
                    {
                        LogFail(GET_COMPANY_OPERATION, response.StatusCode, "Error when parse company");

                        return ServiceResponse<Company?>.Failure("Не удалось получить компанию", response.StatusCode);
                    }

                    return ServiceResponse<Company?>.Success(company);
                },
                operationName: GET_COMPANY_OPERATION
            );
        }

        public async Task<ServiceResponse<ListDataResponse<User>>> GetCompanyMembersAsync(
            Guid companyId,
            int page,
            int pageSize = 20,
            string? searchText = null
        )
        {
            string path = $"{_companyPath}/{companyId}/members{AddQuery("page", page, false)}{AddQuery("page_size", pageSize)}";

            if (!string.IsNullOrWhiteSpace(searchText))
                path += AddQuery("search_text", searchText);

            return await ExecuteApiCallAsync(
                apiCall: () => _httpClient.GetAsync(path),
                successHandler: async response =>
                {
                    var members = await response.Content.ReadFromJsonAsync<ListDataResponse<User>>(Settings.UserJsonOptions);
                    if (members is null)
                    {
                        LogFail(GET_COMPANY_MEMBERS_OPERATION, response.StatusCode, "Error when parse company members");

                        return ServiceResponse<ListDataResponse<User>>.Failure("Не удалось получить участников компании", response.StatusCode);
                    }

                    return ServiceResponse<ListDataResponse<User>>.Success(members);
                },
                operationName: GET_COMPANY_MEMBERS_OPERATION
            );
        }

        public async Task<ServiceResponse<Company?>> CreateCompanyAsync(
            string name, Guid ownerId, IEnumerable<Guid> members
        )
        {
            var content = SerializeData(new { Name = name, OwnerId = ownerId, Members = members });

            return await ExecuteApiCallAsync(
                apiCall: () => _httpClient.PostAsync(_companyPath, content),
                successHandler: async response =>
                {
                    var company = await response.Content.ReadFromJsonAsync<Company>(Settings.UserJsonOptions);
                    if (company is null)
                    {
                        LogFail(CREATE_COMPANY_OPERATION, response.StatusCode, "Error when create company");

                        return ServiceResponse<Company?>.Failure("Не удалось создать компанию", response.StatusCode);
                    }

                    return ServiceResponse<Company?>.Success(company);
                },
                operationName: CREATE_COMPANY_OPERATION
            );
        }

        public async Task<ServiceResponse<Company?>> UpdateCompanyAsync(
            Guid companyId,
            string? name = null,
            Guid? ownerId = null,
            IEnumerable<Guid>? newMembersIds = null,
            IEnumerable<Guid>? removeMembersIds = null
        )
        {
            var content = SerializeData(new { Id = companyId, Name = name, OwnerId = ownerId, NewMembersIds = newMembersIds, RemoveMembersIds = removeMembersIds });

            return await ExecuteApiCallAsync(
                apiCall: () => _httpClient.PutAsync(_companyPath, content),
                successHandler: async response =>
                {
                    var company = await response.Content.ReadFromJsonAsync<Company>(Settings.UserJsonOptions);
                    if (company is null)
                    {
                        LogFail(UPDATE_COMPANY_OPERATION, response.StatusCode, "Error when update company");

                        return ServiceResponse<Company?>.Failure("Не удалось обновить компанию", response.StatusCode);
                    }

                    return ServiceResponse<Company?>.Success(company);
                },
                operationName: UPDATE_COMPANY_OPERATION
            );
        }

        public async Task<ServiceResponse<string>> DeleteCompanyAsync(Guid companyId)
        {
            string path = $"{_companyPath}/{companyId}";

            return await ExecuteApiCallAsync(
                apiCall: () => _httpClient.DeleteAsync(path),
                successHandler: async response =>
                {
                    var message = await response.Content.ReadFromJsonAsync<MessageResponse>(Settings.BaseJsonOptions);
                    if (message is null)
                    {
                        LogFail(DELETE_COMPANY_OPERATION, response.StatusCode, "Error when delete company");

                        return ServiceResponse<string>.Failure("Не удалось удалить компанию", response.StatusCode);
                    }

                    return ServiceResponse<string>.Success(message.Message);
                },
                operationName: DELETE_COMPANY_OPERATION
            );
        }
    }
}
