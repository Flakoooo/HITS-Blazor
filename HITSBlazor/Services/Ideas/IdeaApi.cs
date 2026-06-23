using HITSBlazor.Models.Common.Entities;
using HITSBlazor.Models.Common.Responses;
using HITSBlazor.Models.Ideas.Entities;
using HITSBlazor.Models.Ideas.Enums;
using HITSBlazor.Pages.Ideas.IdeasCreate;
using HITSBlazor.Utils;
using HITSBlazor.Utils.EnumUIConverters;
using System.Net.Http.Json;

namespace HITSBlazor.Services.Ideas
{
    public class IdeaApi(
        IHttpClientFactory httpClientFactory, 
        ILogger<IdeaApi> logger
    ) : BaseApiService(httpClientFactory.CreateClient(Settings.HttpClientName), logger)
    {
        private readonly string _ideaPath = "/api/idea";
        private readonly string _ideasSkillsPath = $"/api/idea/skills";


        private const string GET_IDEAS_OPERATION = "GetIdeas";
        private const string GET_IDEA_OPERATION = "GetIdea";
        private const string CREATE_IDEA_OPERATION = "CreateIdea";
        private const string UPDATE_IDEA_OPERATION = "UpdateIdea";
        private const string UPDATE_IDEA_STATUS_OPERATION = "UpdateIdeaStatus";
        private const string DELETE_IDEA_OPERATION = "DeleteIdea";

        private const string GET_IDEAS_SKILLS_OPERATION = "GetIdeasSkills";
        private const string UPDATE_IDEAS_SKILLS_OPERATION = "UpdateIdeasSkills";

        //IDEAS

        public async Task<ServiceResponse<ListDataResponse<Idea>>> GetIdeasAsync(
            int page,
            int pageSize = 20,
            string? searchText = null,
            HashSet<IdeaStatusType>? statusTypes = null
        )
        {
            string path = $"{_ideaPath}{AddQuery("page", page, false)}{AddQuery("page_size", pageSize)}";

            if (!string.IsNullOrWhiteSpace(searchText))
                path += AddQuery("search_text", searchText);

            if (statusTypes?.Count > 0)
                path += AddQuery("status_type", statusTypes);

            return await ExecuteApiCallAsync(
                apiCall: () => _httpClient.GetAsync(path),
                successHandler: async response =>
                {
                    var ideas = await response.Content.ReadFromJsonAsync<ListDataResponse<Idea>>(Settings.BaseJsonOptions);
                    if (ideas is null)
                    {
                        LogFail(GET_IDEAS_OPERATION, response.StatusCode, "Error when parse ideas");

                        return ServiceResponse<ListDataResponse<Idea>>.Failure("Не удалось получить идеи", response.StatusCode);
                    }

                    return ServiceResponse<ListDataResponse<Idea>>.Success(ideas);
                },
                operationName: GET_IDEAS_OPERATION
            );
        }

        public async Task<ServiceResponse<Idea?>> GetIdeaAsync(Guid ideaId)
        {
            string path = $"{_ideaPath}/{ideaId}";

            return await ExecuteApiCallAsync(
                apiCall: () => _httpClient.GetAsync(path),
                successHandler: async response =>
                {
                    var idea = await response.Content.ReadFromJsonAsync<Idea>(Settings.BaseJsonOptions);
                    if (idea is null)
                    {
                        LogFail(GET_IDEA_OPERATION, response.StatusCode, "Error when parse idea");

                        return ServiceResponse<Idea?>.Failure("Не удалось получить идею", response.StatusCode);
                    }

                    return ServiceResponse<Idea?>.Success(idea);
                },
                operationName: GET_IDEA_OPERATION
            );
        }

        public async Task<ServiceResponse<Idea?>> CreateIdeaAsync(IdeasCreateModel ideasCreateModel)
        {
            var content = SerializeData(ideasCreateModel);

            return await ExecuteApiCallAsync(
                apiCall: () => _httpClient.PostAsync(_ideaPath, content),
                successHandler: async response =>
                {
                    var idea = await response.Content.ReadFromJsonAsync<Idea>(Settings.BaseJsonOptions);
                    if (idea is null)
                    {
                        LogFail(CREATE_IDEA_OPERATION, response.StatusCode, "Error when create idea");

                        return ServiceResponse<Idea?>.Failure("Не удалось создать идею", response.StatusCode);
                    }

                    return ServiceResponse<Idea?>.Success(idea);
                },
                operationName: CREATE_IDEA_OPERATION
            );
        }

        public async Task<ServiceResponse<Idea?>> UpdateIdeaAsync(IdeasCreateModel ideasCreateModel)
        {
            var content = SerializeData(ideasCreateModel);

            return await ExecuteApiCallAsync(
                apiCall: () => _httpClient.PostAsync(_ideaPath, content),
                successHandler: async response =>
                {
                    var idea = await response.Content.ReadFromJsonAsync<Idea>(Settings.BaseJsonOptions);
                    if (idea is null)
                    {
                        LogFail(UPDATE_IDEA_OPERATION, response.StatusCode, "Error when update idea");

                        return ServiceResponse<Idea?>.Failure("Не удалось обновить идею", response.StatusCode);
                    }

                    return ServiceResponse<Idea?>.Success(idea);
                },
                operationName: UPDATE_IDEA_OPERATION
            );
        }

        public async Task<ServiceResponse<string>> UpdateIdeaStatusAsync(Guid ideaId, IdeaStatusType newStatus)
        {
            string path = $"{_ideaPath}/status";
            var content = SerializeData(new { Id = ideaId, Status = newStatus });

            return await ExecuteApiCallAsync(
                apiCall: () => _httpClient.PutAsync(path, content),
                successHandler: async response =>
                {
                    var message = await response.Content.ReadFromJsonAsync<MessageResponse>(Settings.BaseJsonOptions);
                    if (message is null)
                    {
                        LogFail(UPDATE_IDEA_STATUS_OPERATION, response.StatusCode, "Error when update idea status");

                        return ServiceResponse<string>.Failure($"Не удалось сменить статус идеи на {EnumUIConverter.GetInfo(newStatus)}", response.StatusCode);
                    }

                    return ServiceResponse<string>.Success(message.Message);
                },
                operationName: UPDATE_IDEA_STATUS_OPERATION
            );
        }

        public async Task<ServiceResponse<string>> DeleteIdeaAsync(Guid ideaId)
        {
            string path = $"{_ideaPath}/{ideaId}";

            return await ExecuteApiCallAsync(
                apiCall: () => _httpClient.DeleteAsync(path),
                successHandler: async response =>
                {
                    var message = await response.Content.ReadFromJsonAsync<MessageResponse>(Settings.BaseJsonOptions);
                    if (message is null)
                    {
                        LogFail(DELETE_IDEA_OPERATION, response.StatusCode, "Error when delete idea");

                        return ServiceResponse<string>.Failure("Не удалось удалить идею", response.StatusCode);
                    }

                    return ServiceResponse<string>.Success(message.Message);
                },
                operationName: DELETE_IDEA_OPERATION
            );
        }

        // IDEA SKILLS
        public async Task<ServiceResponse<List<Skill>>> GetIdeasSkillsAsync(Guid ideaId)
        {
            string path = $"{_ideasSkillsPath}/{ideaId}";

            return await ExecuteApiCallAsync(
                apiCall: () => _httpClient.GetAsync(path),
                successHandler: async response =>
                {
                    var skills = await response.Content.ReadFromJsonAsync<List<Skill>>(Settings.BaseJsonOptions);
                    if (skills is null)
                    {
                        LogFail(GET_IDEAS_SKILLS_OPERATION, response.StatusCode, "Error when parse ideas skills");

                        return ServiceResponse<List<Skill>>.Failure("Не удалось получить комптенции идеи", response.StatusCode);
                    }

                    return ServiceResponse<List<Skill>>.Success(skills);
                },
                operationName: GET_IDEAS_SKILLS_OPERATION
            );
        }

        public async Task<ServiceResponse<string>> UpdateIdeasSkillsAsync(Guid ideaId, List<Skill> newSkills)
        {
            var content = SerializeData(new { Id = ideaId, Skills = newSkills });

            return await ExecuteApiCallAsync(
                apiCall: () => _httpClient.PostAsync(_ideasSkillsPath, content),
                successHandler: async response =>
                {
                    var message = await response.Content.ReadFromJsonAsync<MessageResponse>(Settings.BaseJsonOptions);
                    if (message is null)
                    {
                        LogFail(UPDATE_IDEAS_SKILLS_OPERATION, response.StatusCode, "Error when update ideas skills");

                        return ServiceResponse<string>.Failure("Не удалось обновить комптенции идеи", response.StatusCode);
                    }

                    return ServiceResponse<string>.Success(message.Message);
                },
                operationName: UPDATE_IDEAS_SKILLS_OPERATION
            );
        }
    }
}
