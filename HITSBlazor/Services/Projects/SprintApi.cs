using HITSBlazor.Models.Common.Responses;
using HITSBlazor.Models.Projects.Entities;
using HITSBlazor.Models.Projects.Requests;
using HITSBlazor.Utils;
using System.Net.Http.Json;

namespace HITSBlazor.Services.Projects
{
    public class SprintApi(
        IHttpClientFactory httpClientFactory,
        ILogger<SprintApi> logger
    ) : BaseApiService(httpClientFactory.CreateClient(Settings.HttpClientName), logger)
    {
        private readonly string _sprintPath = "/api/sprint";

        private const string GET_SPRINTS_OPERATION = "GetSprints";
        private const string GET_SPRINT_OPERATION = "GetSprint";
        private const string GET_ACTIVE_SPRINT_OPERATION = "GetActiveSprint";
        private const string CREATE_SPRINT_OPERATION = "CreateProject";
        private const string UPDATE_SPRINT_OPERATION = "UpdateProject";
        private const string FINISH_SPRINT_OPERATION = "FinishProject";

        private const string GET_SPRINT_MARKS_OPERATION = "GetSprintMarks";
        private const string CREATE_SPRINT_MARKS_OPERATION = "CreateSprintMarks";

        public async Task<ServiceResponse<ListDataResponse<Sprint>>> GetSprintsAsync(
            Guid projectId,
            int page,
            int pageSize = 20,
            string? searchText = null
        )
        {
            string path = $"{_sprintPath}/project/{projectId}{AddQuery("page", page, false)}{AddQuery("page_size", pageSize)}";

            if (!string.IsNullOrWhiteSpace(searchText))
                path += AddQuery("search_text", searchText);

            return await ExecuteApiCallAsync(
                apiCall: () => _httpClient.GetAsync(path),
                successHandler: async response =>
                {
                    var sprints = await response.Content.ReadFromJsonAsync<ListDataResponse<Sprint>>(Settings.BaseJsonOptions);
                    if (sprints is null)
                    {
                        LogFail(GET_SPRINTS_OPERATION, response.StatusCode, "Error when parse sprints");

                        return ServiceResponse<ListDataResponse<Sprint>>.Failure("Не удалось получить спринты", response.StatusCode);
                    }

                    return ServiceResponse<ListDataResponse<Sprint>>.Success(sprints);
                },
                operationName: GET_SPRINTS_OPERATION
            );
        }

        public async Task<ServiceResponse<Sprint?>> GetSprintAsync(Guid sprintId)
        {
            string path = $"{_sprintPath}/{sprintId}";

            return await ExecuteApiCallAsync(
                apiCall: () => _httpClient.GetAsync(path),
                successHandler: async response =>
                {
                    var sprint = await response.Content.ReadFromJsonAsync<Sprint>(Settings.BaseJsonOptions);
                    if (sprint is null)
                    {
                        LogFail(GET_SPRINT_OPERATION, response.StatusCode, "Error when parse sprint");

                        return ServiceResponse<Sprint?>.Failure("Не удалось получить спринт", response.StatusCode);
                    }

                    return ServiceResponse<Sprint?>.Success(sprint);
                },
                operationName: GET_SPRINT_OPERATION
            );
        }

        public async Task<ServiceResponse<Sprint?>> GetActiveSprintAsync(Guid projectId)
        {
            string path = $"{_sprintPath}/project/{projectId}/active";

            return await ExecuteApiCallAsync(
                apiCall: () => _httpClient.GetAsync(path),
                successHandler: async response =>
                {
                    var sprint = await response.Content.ReadFromJsonAsync<Sprint>(Settings.BaseJsonOptions);
                    if (sprint is null)
                    {
                        LogFail(GET_ACTIVE_SPRINT_OPERATION, response.StatusCode, "Error when parse sprint");

                        return ServiceResponse<Sprint?>.Failure("Не удалось получить активный спринт", response.StatusCode);
                    }

                    return ServiceResponse<Sprint?>.Success(sprint);
                },
                operationName: GET_ACTIVE_SPRINT_OPERATION
            );
        }

        public async Task<ServiceResponse<Sprint?>> CreateSprintAsync(CreateSprintRequest request)
        {
            string path = $"{_sprintPath}/add";
            var content = SerializeData(request);

            return await ExecuteApiCallAsync(
                apiCall: () => _httpClient.PostAsync(path, content),
                successHandler: async response =>
                {
                    var sprint = await response.Content.ReadFromJsonAsync<Sprint>(Settings.BaseJsonOptions);
                    if (sprint is null)
                    {
                        LogFail(CREATE_SPRINT_OPERATION, response.StatusCode, "Error when create sprint");

                        return ServiceResponse<Sprint?>.Failure("Не удалось создать спринт", response.StatusCode);
                    }

                    return ServiceResponse<Sprint?>.Success(sprint);
                },
                operationName: CREATE_SPRINT_OPERATION
            );
        }

        public async Task<ServiceResponse<Sprint?>> UpdateSprintAsync(Guid sprintId, UpdateSprintRequest request)
        {
            string path = $"{_sprintPath}/{sprintId}/update";
            var content = SerializeData(request);

            return await ExecuteApiCallAsync(
                apiCall: () => _httpClient.PutAsync(path, content),
                successHandler: async response =>
                {
                    var sprint = await response.Content.ReadFromJsonAsync<Sprint>(Settings.BaseJsonOptions);
                    if (sprint is null)
                    {
                        LogFail(UPDATE_SPRINT_OPERATION, response.StatusCode, "Error when update sprint");

                        return ServiceResponse<Sprint?>.Failure("Не удалось обновить спринт", response.StatusCode);
                    }

                    return ServiceResponse<Sprint?>.Success(sprint);
                },
                operationName: UPDATE_SPRINT_OPERATION
            );
        }

        public async Task<ServiceResponse<string>> FinishSprintAsync(Guid sprintId, string report)
        {
            string path = $"{_sprintPath}/{sprintId}/finish";
            var content = SerializeData(report);

            return await ExecuteApiCallAsync(
                apiCall: () => _httpClient.PutAsync(path, content),
                successHandler: async response =>
                {
                    var message = await response.Content.ReadFromJsonAsync<MessageResponse>(Settings.BaseJsonOptions);
                    if (message is null)
                    {
                        LogFail(FINISH_SPRINT_OPERATION, response.StatusCode, "Error when finish sprint");

                        return ServiceResponse<string>.Failure("Не удалось завершить спринт", response.StatusCode);
                    }

                    return ServiceResponse<string>.Success(message.Message);
                },
                operationName: FINISH_SPRINT_OPERATION
            );
        }

        //SPRINT MARKS
        public async Task<ServiceResponse<IEnumerable<SprintMarks>>> GetSprintMarksAsync(Guid sprintId)
        {
            string path = $"{_sprintPath}/marks/{sprintId}/all";

            return await ExecuteApiCallAsync(
                apiCall: () => _httpClient.GetAsync(path),
                successHandler: async response =>
                {
                    var marks = await response.Content.ReadFromJsonAsync<IEnumerable<SprintMarks>>(Settings.BaseJsonOptions);
                    if (marks is null)
                    {
                        LogFail(GET_SPRINT_MARKS_OPERATION, response.StatusCode, "Error when parse sprint marks");

                        return ServiceResponse<IEnumerable<SprintMarks>>.Failure("Не удалось получить оценки спринта", response.StatusCode);
                    }

                    return ServiceResponse<IEnumerable<SprintMarks>>.Success(marks);
                },
                operationName: GET_SPRINT_MARKS_OPERATION
            );
        }

        public async Task<ServiceResponse<string>> CreateSprintMarksAsync(Guid sprintId, IEnumerable<SprintMarkRequest> marks)
        {
            string path = $"{_sprintPath}/marks/{sprintId}/add";
            var content = SerializeData(marks);

            return await ExecuteApiCallAsync(
                apiCall: () => _httpClient.PostAsync(path, content),
                successHandler: async response =>
                {
                    var message = await response.Content.ReadFromJsonAsync<MessageResponse>(Settings.BaseJsonOptions);
                    if (message is null)
                    {
                        LogFail(CREATE_SPRINT_MARKS_OPERATION, response.StatusCode, "Error when create sprint marks");

                        return ServiceResponse<string>.Failure("Не удалось создать оценки спринта", response.StatusCode);
                    }

                    return ServiceResponse<string>.Success(message.Message);
                },
                operationName: CREATE_SPRINT_MARKS_OPERATION
            );
        }
    }
}
