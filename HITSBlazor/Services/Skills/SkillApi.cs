using ApexCharts;
using HITSBlazor.Components.Skills;
using HITSBlazor.Models.Common.Entities;
using HITSBlazor.Models.Common.Enums;
using HITSBlazor.Models.Common.Responses;
using HITSBlazor.Utils;
using System.Net.Http.Json;

namespace HITSBlazor.Services.Skills
{
    public class SkillApi(
        IHttpClientFactory httpClientFactory,
        ILogger<SkillApi> logger
    ) : BaseApiService(httpClientFactory.CreateClient(Settings.HttpClientName), logger)
    {
        private readonly string _skillPath = "api/skill";

        private const string GET_SKILLS_OPERATION = "GetSkills";
        private const string CREATE_SKILL_OPERATION = "CreateSkill";
        private const string UPDATE_SKILL_OPERATION = "UpdateSkill";
        private const string DELETE_SKILL_OPERATION = "DeleteSkill";

        public async Task<ServiceResponse<ListDataResponse<Skill>>> GetSkillsAsync(
            int page,
            int pageSize = 20,
            string? searchText = null,
            bool? confirmed = null,
            IEnumerable<SkillType>? skillTypes = null
        )
        {
            string path = $"{_skillPath}{AddQuery("page", page, false)}{AddQuery("page_size", pageSize)}";

            if (!string.IsNullOrWhiteSpace(searchText))
                path += AddQuery("search_text", searchText);

            if (confirmed.HasValue)
                path += AddQuery("confirmed", confirmed.Value);

            if (skillTypes is not null && skillTypes.Any())
                path += AddQuery("skill_types", skillTypes);

            return await ExecuteApiCallAsync(
                apiCall: () => _httpClient.GetAsync(path),
                successHandler: async response => 
                {
                    var skills = await response.Content.ReadFromJsonAsync<ListDataResponse<Skill>>(Settings.BaseJsonOptions);
                    if (skills is null)
                    {
                        LogFail(GET_SKILLS_OPERATION, response.StatusCode, "Error when parse skills");

                        return ServiceResponse<ListDataResponse<Skill>>.Failure("Не удалось получить компетенции", response.StatusCode);
                    }

                    return ServiceResponse<ListDataResponse<Skill>>.Success(skills);
                },
                operationName: GET_SKILLS_OPERATION
            );
        }

        public async Task<ServiceResponse<Skill?>> CreateSkillAsync(string name, SkillType type)
        {
            var content = SerializeData(new { Name = name, Type = type });

            return await ExecuteApiCallAsync(
                apiCall: () => _httpClient.PostAsync(_skillPath, content),
                successHandler: async response =>
                {
                    var skill = await response.Content.ReadFromJsonAsync<Skill>(Settings.BaseJsonOptions);
                    if (skill is null)
                    {
                        LogFail(CREATE_SKILL_OPERATION, response.StatusCode, "Error when create skill");

                        return ServiceResponse<Skill?>.Failure("Не удалось создать компетенцию", response.StatusCode);
                    }

                    return ServiceResponse<Skill?>.Success(skill);
                },
                operationName: CREATE_SKILL_OPERATION
            );
        }

        public async Task<ServiceResponse<string>> UpdateSkillAsync(Guid id, string name, SkillType type, bool confirmed)
        {
            var content = SerializeData(new { 
                Id = id,
                Name = name,
                Type = type,
                Confirmed = confirmed
            });

            return await ExecuteApiCallAsync(
                apiCall: () => _httpClient.PutAsync(_skillPath, content),
                successHandler: async response =>
                {
                    var message = await response.Content.ReadFromJsonAsync<MessageResponse>(Settings.BaseJsonOptions);
                    if (message is null)
                    {
                        LogFail(UPDATE_SKILL_OPERATION, response.StatusCode, "Error when update skill");

                        return ServiceResponse<string>.Failure("Не удалось обновить компетенцию", response.StatusCode);
                    }

                    return ServiceResponse<string>.Success(message.Message);
                },
                operationName: UPDATE_SKILL_OPERATION
            );
        }

        public async Task<ServiceResponse<string>> DeleteSkillAsync(Guid id)
        {
            string path = $"{_skillPath}/{id}";

            return await ExecuteApiCallAsync(
                apiCall: () => _httpClient.DeleteAsync(path),
                successHandler: async response =>
                {
                    var message = await response.Content.ReadFromJsonAsync<MessageResponse>(Settings.BaseJsonOptions);
                    if (message is null)
                    {
                        LogFail(DELETE_SKILL_OPERATION, response.StatusCode, "Error when delete skill");

                        return ServiceResponse<string>.Failure("Не удалось удалить компетенцию", response.StatusCode);
                    }

                    return ServiceResponse<string>.Success(message.Message);
                },
                operationName: DELETE_SKILL_OPERATION
            );
        }
    }
}
