using HITSBlazor.Models.Common.Entities;
using HITSBlazor.Models.Common.Responses;
using HITSBlazor.Utils;
using System.Net.Http.Json;

namespace HITSBlazor.Services.UserSkills
{
    public class UserSkillsApi(
        IHttpClientFactory httpClientFactory,
        ILogger<UserSkillsApi> logger
    ) : BaseApiService(httpClientFactory.CreateClient(Settings.HttpClientName), logger)
    {
        private readonly string _userSkillPath = "/api/profile";

        private const string GET_USER_SKILLS_OPERATION = "GetUserSkills";
        private const string UPDATE_USER_SKILLS_OPERATION = "UpdateUserSkills";

        public async Task<ServiceResponse<List<Skill>>> GetUserSkillsAsync(Guid? userId = null)
        {
            string path = $"{_userSkillPath}/skills{(userId.HasValue ? $"/{userId}" : string.Empty)}";

            return await ExecuteApiCallAsync(
                apiCall: () => _httpClient.GetAsync(path),
                successHandler: async resposne =>
                {
                    var skills = await resposne.Content.ReadFromJsonAsync<List<Skill>>(Settings.SkillJsonOptions);
                    if (skills is null)
                    {
                        LogFail(GET_USER_SKILLS_OPERATION, resposne.StatusCode, "Error when parse user skills");

                        return ServiceResponse<List<Skill>>.Failure("Не удалось получить компетенции пользователя", resposne.StatusCode);
                    }

                    return ServiceResponse<List<Skill>>.Success(skills);
                },
                operationName: GET_USER_SKILLS_OPERATION
            );
        }

        public async Task<ServiceResponse<string>> UpdateUserSkillsAsync(IEnumerable<Guid> skillsIds)
        {
            string path = $"{_userSkillPath}/skills";
            var content = SerializeData(skillsIds);

            return await ExecuteApiCallAsync(
                apiCall: () => _httpClient.PutAsync(path, content),
                successHandler: async resposne =>
                {
                    var message = await resposne.Content.ReadFromJsonAsync<MessageResponse>(Settings.BaseJsonOptions);
                    if (message is null)
                    {
                        LogFail(UPDATE_USER_SKILLS_OPERATION, resposne.StatusCode, "Error when update user skills");

                        return ServiceResponse<string>.Failure("Не удалось обновить компетенции", resposne.StatusCode);
                    }

                    return ServiceResponse<string>.Success(message.Message);
                },
                operationName: UPDATE_USER_SKILLS_OPERATION
            );
        }
    }
}
