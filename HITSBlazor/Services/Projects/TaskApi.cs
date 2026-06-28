using ApexCharts;
using HITSBlazor.Models.Common.Responses;
using HITSBlazor.Models.Projects.Entities;
using HITSBlazor.Models.Projects.Enums;
using HITSBlazor.Models.Projects.Requests;
using HITSBlazor.Models.Users.Entities;
using HITSBlazor.Utils;
using System.Net.Http.Json;
using HITSTask = HITSBlazor.Models.Projects.Entities.Task;
using HITSTaskStatus = HITSBlazor.Models.Projects.Enums.TaskStatus;

namespace HITSBlazor.Services.Projects
{
    public class TaskApi(
        IHttpClientFactory httpClientFactory,
        ILogger<TaskApi> logger
    ) : BaseApiService(httpClientFactory.CreateClient(Settings.HttpClientName), logger)
    {
        private readonly string _taskPath = "/api/task";
        private readonly string _taskLogPath = "/api/log";

        private const string GET_TASKS_OPERATION = "GetTasks";
        private const string GET_TASK_OPERATION = "GetTask";
        private const string MEMBER_HAS_TASK_OPERATION = "MemberHasTask";
        private const string CREATE_TASK_OPERATION = "CreateTask";
        private const string UPDATE_TASK_OPERATION = "UpdateTask";
        private const string UPDATE_TASK_COMMENT_OPERATION = "UpdateTaskComment";

        private const string GET_TASK_LOGS_OPERATION = "GetTaskLogs";
        private const string CREATE_TASK_LOG_OPERATION = "CreateTaskLog";

        public async Task<ServiceResponse<ListDataResponse<HITSTask>>> GetTasksAsync(
            int page,
            int pageSize = 20,
            Guid? projectId = null,
            Guid? sprintId = null,
            string? searchText = null,
            IEnumerable<HITSTaskStatus>? selectedStatuses = null,
            IEnumerable<Guid>? selectedTags = null,
            IEnumerable<Guid>? selectedExecutors = null
        )
        {
            string path = $"{_taskPath}/project{AddQuery("page", page, false)}{AddQuery("page_size", pageSize)}";

            if (projectId.HasValue)
                path += AddQuery("project_id", projectId.Value);

            if (sprintId.HasValue)
                path += AddQuery("sprint_id", sprintId.Value);

            if (!string.IsNullOrWhiteSpace(searchText))
                path += AddQuery("search_text", searchText);

            if (selectedStatuses is not null && selectedStatuses.Any())
                path += AddQuery("selected_status", selectedStatuses);

            if (selectedTags is not null && selectedTags.Any())
                path += AddQuery("selected_tag", selectedTags);

            if (selectedExecutors is not null && selectedExecutors.Any())
                path += AddQuery("selected_executor", selectedExecutors);

            return await ExecuteApiCallAsync(
                apiCall: () => _httpClient.GetAsync(path),
                successHandler: async response =>
                {
                    var tasks = await response.Content.ReadFromJsonAsync<ListDataResponse<HITSTask>>(Settings.BaseJsonOptions);
                    if (tasks is null)
                    {
                        LogFail(GET_TASKS_OPERATION, response.StatusCode, "Error when parse tasks");

                        return ServiceResponse<ListDataResponse<HITSTask>>.Failure("Не удалось получить задачи", response.StatusCode);
                    }

                    return ServiceResponse<ListDataResponse<HITSTask>>.Success(tasks);
                },
                operationName: GET_TASKS_OPERATION
            );
        }

        public async Task<ServiceResponse<HITSTask?>> GetTaskAsync(Guid taskId)
        {
            string path = $"{_taskPath}/{taskId}";

            return await ExecuteApiCallAsync(
                apiCall: () => _httpClient.GetAsync(path),
                successHandler: async response =>
                {
                    var task = await response.Content.ReadFromJsonAsync<HITSTask>(Settings.BaseJsonOptions);
                    if (task is null)
                    {
                        LogFail(GET_TASK_OPERATION, response.StatusCode, "Error when parse task");

                        return ServiceResponse<HITSTask?>.Failure("Не удалось получить задачу", response.StatusCode);
                    }

                    return ServiceResponse<HITSTask?>.Success(task);
                },
                operationName: GET_TASK_OPERATION
            );
        }

        public async Task<ServiceResponse<bool>> MemberHasActiveTaskAsync(Guid sprintId)
        {
            string path = $"{_taskPath}/active-task/{sprintId}";

            return await ExecuteApiCallAsync(
                apiCall: () => _httpClient.GetAsync(path),
                successHandler: async response =>
                {
                    return ServiceResponse<bool>.Success(true);
                },
                operationName: MEMBER_HAS_TASK_OPERATION
            );
        }

        public async Task<ServiceResponse<HITSTask?>> CreateTaskAsync(CreateTaskRequest request)
        {
            string path = $"{_taskPath}/add";
            var content = SerializeData(request);

            return await ExecuteApiCallAsync(
                apiCall: () => _httpClient.PostAsync(path, content),
                successHandler: async response =>
                {
                    var task = await response.Content.ReadFromJsonAsync<HITSTask>(Settings.BaseJsonOptions);
                    if (task is null)
                    {
                        LogFail(CREATE_TASK_OPERATION, response.StatusCode, "Error when create task");

                        return ServiceResponse<HITSTask?>.Failure("Не удалось создать задачу", response.StatusCode);
                    }

                    return ServiceResponse<HITSTask?>.Success(task);
                },
                operationName: CREATE_TASK_OPERATION
            );
        }

        public async Task<ServiceResponse<HITSTask?>> UpdateTaskAsync(Guid taskId, UpdateTaskRequest request)
        {
            string path = $"{_taskPath}/update/{taskId}";
            var content = SerializeData(request);

            return await ExecuteApiCallAsync(
                apiCall: () => _httpClient.PutAsync(path, content),
                successHandler: async response =>
                {
                    var task = await response.Content.ReadFromJsonAsync<HITSTask>(Settings.BaseJsonOptions);
                    if (task is null)
                    {
                        LogFail(UPDATE_TASK_OPERATION, response.StatusCode, "Error when update task");

                        return ServiceResponse<HITSTask?>.Failure("Не удалось изменить задачу", response.StatusCode);
                    }

                    return ServiceResponse<HITSTask?>.Success(task);
                },
                operationName: UPDATE_TASK_OPERATION
            );
        }

        public async Task<ServiceResponse<string>> UpdateTaskCommentAsync(Guid taskId, Guid userId, string comment, ProjectMemberRole executorRole)
        {
            string path = $"{_taskPath}/{(executorRole )}/comment/{taskId}";
            path = executorRole switch
            {
                ProjectMemberRole.Member => $"{_taskPath}/executor/{taskId}/{userId}",
                ProjectMemberRole.TeamLeader => $"{_taskPath}/leader/comment/{taskId}",
                _ => string.Empty
            };

            if (string.IsNullOrWhiteSpace(path))
                return ServiceResponse<string>.Failure("Не удалось изменить комментарий задачи");

            var content = SerializeData(comment);

            return await ExecuteApiCallAsync(
                apiCall: () => _httpClient.PutAsync(path, content),
                successHandler: async response =>
                {
                    var message = await response.Content.ReadFromJsonAsync<MessageResponse>(Settings.BaseJsonOptions);
                    if (message is null)
                    {
                        LogFail(UPDATE_TASK_COMMENT_OPERATION, response.StatusCode, "Error when change task comment");

                        return ServiceResponse<string>.Failure("Не удалось изменить комментарий задачи", response.StatusCode);
                    }

                    return ServiceResponse<string>.Success(message.Message);
                },
                operationName: UPDATE_TASK_COMMENT_OPERATION
            );
        }

        //TASK LOGS
        public async Task<ServiceResponse<ListDataResponse<TaskMovementLog>>> GetTasksLogsAsync(
            Guid taskId,
            int page,
            int pageSize = 20
        )
        {
            string path = $"{_taskLogPath}/all/{taskId}{AddQuery("page", page, false)}{AddQuery("page_size", pageSize)}";

            return await ExecuteApiCallAsync(
                apiCall: () => _httpClient.GetAsync(path),
                successHandler: async response =>
                {
                    var logs = await response.Content.ReadFromJsonAsync<ListDataResponse<TaskMovementLog>>(Settings.BaseJsonOptions);
                    if (logs is null)
                    {
                        LogFail(GET_TASK_LOGS_OPERATION, response.StatusCode, "Error when parse tasks log");

                        return ServiceResponse<ListDataResponse<TaskMovementLog>>.Failure("Не удалось получить историю задач", response.StatusCode);
                    }

                    return ServiceResponse<ListDataResponse<TaskMovementLog>>.Success(logs);
                },
                operationName: GET_TASK_LOGS_OPERATION
            );
        }

        public async Task<ServiceResponse<TaskMovementLog?>> CreateTasksLogAsync(
            Guid taskId,
            Guid executorId,
            Guid userId,
            HITSTaskStatus newStatus
        )
        {
            string path = $"{_taskLogPath}/add";
            var content = SerializeData(new 
            {
                TaskId = taskId,
                ExecutorId = executorId,
                UserId = userId,
                Status = newStatus
            });

            return await ExecuteApiCallAsync(
                apiCall: () => _httpClient.PostAsync(path, content),
                successHandler: async response =>
                {
                    var log = await response.Content.ReadFromJsonAsync<TaskMovementLog>(Settings.BaseJsonOptions);
                    if (log is null)
                    {
                        LogFail(CREATE_TASK_LOG_OPERATION, response.StatusCode, "Error when create task log");

                        return ServiceResponse<TaskMovementLog?>.Failure("Не удалось передвинуть задачу", response.StatusCode);
                    }

                    return ServiceResponse<TaskMovementLog?>.Success(log);
                },
                operationName: CREATE_TASK_LOG_OPERATION
            );
        }
    }
}
