using HITSBlazor.Models.Common.Responses;
using HITSBlazor.Models.Projects.Entities;
using HITSBlazor.Models.Projects.Enums;
using HITSBlazor.Models.Users.Entities;
using HITSBlazor.Models.Users.Enums;
using HITSBlazor.Utils;
using System.Net.Http.Json;

namespace HITSBlazor.Services.Projects
{
    public class ProjectApi(
        IHttpClientFactory httpClientFactory,
        ILogger<ProjectApi> logger
    ) : BaseApiService(httpClientFactory.CreateClient(Settings.HttpClientName), logger)
    {
        private readonly string _projectPath = "/api/project";

        private const string GET_PROJECTS_OPERATION = "GetProjects";
        private const string GET_ALL_USER_ACTIVE_PROJECTS_OPERATION = "GetAllUserActiveProjects";
        private const string GET_PROJECT_OPERATION = "GetProject";
        private const string CREATE_PROJECT_OPERATION = "CreateProject";

        private const string GET_PROJECT_MARKS_OPERATION = "GetProjectMarks";

        public async Task<ServiceResponse<ListDataResponse<Project>>> GetProjectsAsync(
            User currentUser,
            int page,
            int pageSize = 20,
            string? searchText = null,
            ProjectStatus? selectedStatus = null
        )
        {
            if (!currentUser.Role.HasValue)
                return ServiceResponse<ListDataResponse<Project>>.Failure("Не удалось получить проекты");

            var teamRoles = new HashSet<RoleType>
            {
                RoleType.Initiator,
                RoleType.Member,
                RoleType.TeamLeader
            };

            string path = $"{_projectPath}{(teamRoles.Contains(currentUser.Role.Value) ? "/my" : string.Empty)}{AddQuery("page", page, false)}{AddQuery("page_size", pageSize)}";

            if (!string.IsNullOrWhiteSpace(searchText))
                path += AddQuery("search_text", searchText);

            if (selectedStatus.HasValue)
                path += AddQuery("selected_status", selectedStatus.Value);

            return await ExecuteApiCallAsync(
                apiCall: () => _httpClient.GetAsync(path),
                successHandler: async response =>
                {
                    var projects = await response.Content.ReadFromJsonAsync<ListDataResponse<Project>>(Settings.BaseJsonOptions);
                    if (projects is null)
                    {
                        LogFail(GET_PROJECTS_OPERATION, response.StatusCode, "Error when parse projects");

                        return ServiceResponse<ListDataResponse<Project>>.Failure("Не удалось получить проекты", response.StatusCode);
                    }

                    return ServiceResponse<ListDataResponse<Project>>.Success(projects);
                },
                operationName: GET_PROJECTS_OPERATION
            );
        }

        public async Task<ServiceResponse<IEnumerable<Project>>> GetUserActiveProjectsAsync()
        {
            string path = $"{_projectPath}/my/active";

            return await ExecuteApiCallAsync(
                apiCall: () => _httpClient.GetAsync(path),
                successHandler: async response =>
                {
                    var projects = await response.Content.ReadFromJsonAsync<IEnumerable<Project>>(Settings.BaseJsonOptions);
                    if (projects is null)
                    {
                        LogFail(GET_ALL_USER_ACTIVE_PROJECTS_OPERATION, response.StatusCode, "Error when parse projects");

                        return ServiceResponse<IEnumerable<Project>>.Failure("Не удалось получить проекты", response.StatusCode);
                    }

                    return ServiceResponse<IEnumerable<Project>>.Success(projects);
                },
                operationName: GET_ALL_USER_ACTIVE_PROJECTS_OPERATION
            );
        }

        public async Task<ServiceResponse<Project?>> GetProjectAsync(Guid projectId)
        {
            string path = $"{_projectPath}/{projectId}";

            return await ExecuteApiCallAsync(
                apiCall: () => _httpClient.GetAsync(path),
                successHandler: async response =>
                {
                    var project = await response.Content.ReadFromJsonAsync<Project>(Settings.BaseJsonOptions);
                    if (project is null)
                    {
                        LogFail(GET_PROJECT_OPERATION, response.StatusCode, "Error when parse project");

                        return ServiceResponse<Project?>.Failure("Не удалось получить проект", response.StatusCode);
                    }

                    return ServiceResponse<Project?>.Success(project);
                },
                operationName: GET_PROJECT_OPERATION
            );
        }

        public async Task<ServiceResponse<Project?>> CreateNewProjectAsync(Guid ideaMarketId)
        {
            string path = $"{_projectPath}/create/{ideaMarketId}";

            return await ExecuteApiCallAsync(
                apiCall: () => _httpClient.PostAsync(path, null),
                successHandler: async response =>
                {
                    var project = await response.Content.ReadFromJsonAsync<Project>(Settings.BaseJsonOptions);
                    if (project is null)
                    {
                        LogFail(CREATE_PROJECT_OPERATION, response.StatusCode, "Error when create project");

                        return ServiceResponse<Project?>.Failure("Не удалось создать проект", response.StatusCode);
                    }

                    return ServiceResponse<Project?>.Success(project);
                },
                operationName: CREATE_PROJECT_OPERATION
            );
        }

        public async Task<ServiceResponse<string>> KickProjectMemberAsync(Guid projectId, Guid userId)
        {
            string path = $"{_projectPath}/members/{projectId}/{userId}";

            return await ExecuteApiCallAsync(
                apiCall: () => _httpClient.DeleteAsync(path),
                successHandler: async response =>
                {
                    var message = await response.Content.ReadFromJsonAsync<MessageResponse>(Settings.BaseJsonOptions);
                    if (message is null)
                    {
                        LogFail(CREATE_PROJECT_OPERATION, response.StatusCode, "Error when kick project member");

                        return ServiceResponse<string>.Failure("Не удалось исключить пользоватея", response.StatusCode);
                    }

                    return ServiceResponse<string>.Success(message.Message);
                },
                operationName: CREATE_PROJECT_OPERATION
            );
        }

        private static string GetStatusInfoText(ProjectStatus newStatus, bool isFailed = false) => newStatus switch
        {
            ProjectStatus.Done => isFailed ? "Не удалось завершить проект" : "Проект завершен",
            ProjectStatus.Active => isFailed ? "Не удалось запустить проект" : "Проект запущен",
            ProjectStatus.Paused => isFailed ? "Не удалось остановить проект" : "Проект остановлен",
            ProjectStatus.Deleted => isFailed ? "Не удалось удалить проект" : "Проект удален",
            _ => string.Empty
        };

        public async Task<ServiceResponse<string>> ChangeProjectStatusAsync(Guid projectId, ProjectStatus newStatus, string? report = null)
        {
            string path = newStatus switch
            {
                ProjectStatus.Done => $"{_projectPath}/finish/{projectId}",
                ProjectStatus.Active => $"{_projectPath}/activate/{projectId}",
                ProjectStatus.Paused => $"{_projectPath}/pause/{projectId}",
                ProjectStatus.Deleted => $"{_projectPath}/delete/{projectId}",
                _ => string.Empty
            };

            if (string.IsNullOrWhiteSpace(path))
                return ServiceResponse<string>.Failure(GetStatusInfoText(newStatus));

            return await ExecuteApiCallAsync(
                apiCall: () => _httpClient.PutAsync(path, newStatus switch 
                { 
                    ProjectStatus.Done => SerializeData(new { Report = report }), 
                    _ => null 
                }),
                successHandler: async response =>
                {
                    var message = await response.Content.ReadFromJsonAsync<MessageResponse>(Settings.BaseJsonOptions);
                    if (message is null)
                    {
                        LogFail(CREATE_PROJECT_OPERATION, response.StatusCode, "Error when update project status");

                        return ServiceResponse<string>.Failure(GetStatusInfoText(newStatus), response.StatusCode);
                    }

                    return ServiceResponse<string>.Success(message.Message);
                },
                operationName: $"ChangePorjectStatusTo{newStatus}"
            );
        }

        //ProjectMarks
        public async Task<ServiceResponse<IEnumerable<AverageMark>>> GetProjectMarksAsync(Guid projectId)
        {
            string path = $"{_projectPath}/marks/{projectId}";

            return await ExecuteApiCallAsync(
                apiCall: () => _httpClient.GetAsync(path),
                successHandler: async response =>
                {
                    var marks = await response.Content.ReadFromJsonAsync<IEnumerable<AverageMark>>(Settings.BaseJsonOptions);
                    if (marks is null)
                    {
                        LogFail(GET_PROJECT_MARKS_OPERATION, response.StatusCode, "Error when parse project marks");

                        return ServiceResponse<IEnumerable<AverageMark>>.Failure("Не удалось получить оценки проекта", response.StatusCode);
                    }

                    return ServiceResponse<IEnumerable<AverageMark>>.Success(marks);
                },
                operationName: GET_PROJECT_MARKS_OPERATION
            );
        }
    }
}
