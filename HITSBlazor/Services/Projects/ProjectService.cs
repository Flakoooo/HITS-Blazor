using HITSBlazor.Models.Common.Responses;
using HITSBlazor.Models.Markets.Entities;
using HITSBlazor.Models.Projects.Entities;
using HITSBlazor.Models.Projects.Enums;
using HITSBlazor.Models.Projects.Requests;
using HITSBlazor.Services.Auth;
using HITSBlazor.Utils.Mocks.Projects;
using HITSTask = HITSBlazor.Models.Projects.Entities.Task;
using HITSTaskStatus = HITSBlazor.Models.Projects.Enums.TaskStatus;

namespace HITSBlazor.Services.Projects
{
    public class ProjectService(
        IAuthService authService,
        ProjectApi projectApi,
        SprintApi sprintApi,
        TaskApi taskApi,
        ILogger<ProjectService> logger,
        GlobalNotificationService globalNotificationService
    ) : IProjectService
    {
        private readonly IAuthService _authService = authService;
        private readonly ProjectApi _projectApi = projectApi;
        private readonly SprintApi _sprintApi = sprintApi;
        private readonly TaskApi _taskApi = taskApi;
        private readonly ILogger<ProjectService> _logger = logger;
        private readonly GlobalNotificationService _globalNotificationService = globalNotificationService;

        public event Action<Project>? OnProjectStatusHasChanged;
        public event Action<ProjectMember>? OnMemberHasKicked;

        public event Action<Sprint>? OnSprintHasCreated;
        public event Action<Sprint>? OnSprintHasUpdated;
        public event Action? OnSprintHasFinished;

        public event Action<HITSTask>? OnTaskHasCreated;
        public event Action<HITSTask>? OnTaskHasUpdated;
        public event Action<Guid, string, ProjectMemberRole>? OnTaskCommentUpdated;
        public event Action<HITSTask, HITSTaskStatus>? OnTaskHasMoved;
        public event Action<HITSTask>? OnTaskHasDeleted;

        public async Task<ListDataResponse<Project>> GetProjectsByQueryAsync(
            int page,
            string? searchText,
            ProjectStatus? selectedStatus
        )
        {
            var currentUser = _authService.CurrentUser;
            if (currentUser is null) return new ListDataResponse<Project>(0, []);

            var result = await _projectApi.GetProjectsAsync(
                currentUser, 
                page, 
                searchText: searchText, 
                selectedStatus: selectedStatus
            );

            if (result.IsSuccess && result.Response is not null)
                return result.Response;

            if (!string.IsNullOrWhiteSpace(result.Message))
            {
                _globalNotificationService.ShowError(result.Message);
                if (_logger.IsEnabled(LogLevel.Warning))
                    _logger.LogWarning("Get projects failed: {Error}", result.Message);
            }

            return new ListDataResponse<Project>(0, []);
        }

        public async Task<List<Project>> GetAllActiveProjectsAsync()
        {
            var result = await _projectApi.GetUserActiveProjectsAsync();
            if (result.IsSuccess && result.Response is not null)
                return result.Response.ToList();

            if (!string.IsNullOrWhiteSpace(result.Message))
            {
                _globalNotificationService.ShowError(result.Message);
                if (_logger.IsEnabled(LogLevel.Warning))
                    _logger.LogWarning("Get active user projects failed: {Error}", result.Message);
            }

            return [];
        }

        public async Task<Project?> GetProjectByIdAsync(Guid projectId)
        {
            var result = await _projectApi.GetProjectAsync(projectId);

            if (!string.IsNullOrWhiteSpace(result.Message))
            {
                _globalNotificationService.ShowError(result.Message);
                if (_logger.IsEnabled(LogLevel.Warning))
                    _logger.LogWarning("Get project failed: {Error}", result.Message);
            }

            return result.Response;
        }

        public async Task<bool> CreateNewProjectAsync(IdeaMarket ideaMarket)
        {
            var result = await _projectApi.CreateNewProjectAsync(ideaMarket.Id);
            if (result.IsSuccess && result.Response is not null)
            {
                _globalNotificationService.ShowSuccess("Проект успешно создан");
                return true;
            }  
            
            if (!string.IsNullOrWhiteSpace(result.Message))
            {
                _globalNotificationService.ShowError(result.Message);
                if (_logger.IsEnabled(LogLevel.Warning))
                    _logger.LogWarning("Create project failed: {Error}", result.Message);
            }
            return false;
        }

        //TODOO: реализовать, но как?
        //public async Task<bool> ChangeTeamInProjectAsync()
        //{

        //}

        public async Task<bool> AddMemberInProjectAsync(Guid projectId, Guid userId)
        {
            _globalNotificationService.ShowError("Метод AddMemberInProjectAsync не реализован");
            return false;
        }

        public async Task<bool> KickMemberFromProjectAsync(Guid projectId, ProjectMember member)
        {
            var result = await _projectApi.KickProjectMemberAsync(projectId, member.UserId);
            if (result.IsSuccess && result.Response is not null)
            {
                OnMemberHasKicked?.Invoke(member);
                _globalNotificationService.ShowSuccess(result.Response);
                return true;
            }

            if (!string.IsNullOrWhiteSpace(result.Message))
            {
                _globalNotificationService.ShowError(result.Message);
                if (_logger.IsEnabled(LogLevel.Warning))
                    _logger.LogWarning("Kick project member failed: {Error}", result.Message);
            }
            return false;
        }

        public async Task<bool> ActivateProjectAsync(Project project)
        {
            var result = await _projectApi.ChangeProjectStatusAsync(project.Id, ProjectStatus.Active);
            if (result.IsSuccess && result.Response is not null)
            {
                project.Status = ProjectStatus.Active;
                OnProjectStatusHasChanged?.Invoke(project);
                _globalNotificationService.ShowSuccess(result.Response);
                return true;
            }

            if (!string.IsNullOrWhiteSpace(result.Message))
            {
                _globalNotificationService.ShowError(result.Message);
                if (_logger.IsEnabled(LogLevel.Warning))
                    _logger.LogWarning("Activate project failed: {Error}", result.Message);
            }
            return false;
        }

        public async Task<bool> PauseProjectAsync(Project project)
        {
            var result = await _projectApi.ChangeProjectStatusAsync(project.Id, ProjectStatus.Paused);
            if (result.IsSuccess && result.Response is not null)
            {
                project.Status = ProjectStatus.Paused;
                OnProjectStatusHasChanged?.Invoke(project);
                _globalNotificationService.ShowSuccess(result.Response);
                return true;
            }

            if (!string.IsNullOrWhiteSpace(result.Message))
            {
                _globalNotificationService.ShowError(result.Message);
                if (_logger.IsEnabled(LogLevel.Warning))
                    _logger.LogWarning("Pause project failed: {Error}", result.Message);
            }
            return false;
        }

        public async Task<bool> FinishProjectAsync(Project project, string report)
        {
            var result = await _projectApi.ChangeProjectStatusAsync(project.Id, ProjectStatus.Done, report);
            if (result.IsSuccess && result.Response is not null)
            {
                project.Status = ProjectStatus.Done;
                project.Report = report;
                OnProjectStatusHasChanged?.Invoke(project);
                _globalNotificationService.ShowSuccess(result.Response);
                return true;
            }

            if (!string.IsNullOrWhiteSpace(result.Message))
            {
                _globalNotificationService.ShowError(result.Message);
                if (_logger.IsEnabled(LogLevel.Warning))
                    _logger.LogWarning("Finish project failed: {Error}", result.Message);
            }
            return false;
        }

        public async Task<bool> DeletedProjectAsync(Project project)
        {
            var result = await _projectApi.ChangeProjectStatusAsync(project.Id, ProjectStatus.Deleted);
            if (result.IsSuccess && result.Response is not null)
            {
                project.Status = ProjectStatus.Deleted;
                OnProjectStatusHasChanged?.Invoke(project);
                _globalNotificationService.ShowSuccess(result.Response);
                return true;
            }

            if (!string.IsNullOrWhiteSpace(result.Message))
            {
                _globalNotificationService.ShowError(result.Message);
                if (_logger.IsEnabled(LogLevel.Warning))
                    _logger.LogWarning("Delete project failed: {Error}", result.Message);
            }
            return false;
        }

        //ProjectMarks
        public async Task<List<AverageMark>> GetProjectMarksAsync(Guid projectId)
        {
            var result = await _projectApi.GetProjectMarksAsync(projectId);
            if (result.IsSuccess && result.Response is not null)
                return result.Response.ToList();

            if (!string.IsNullOrWhiteSpace(result.Message))
            {
                _globalNotificationService.ShowError(result.Message);
                if (_logger.IsEnabled(LogLevel.Warning))
                    _logger.LogWarning("Get project marks failed: {Error}", result.Message);
            }

            return [];
        }

        //Sprints
        public async Task<ListDataResponse<Sprint>> GetSprintsByProjectIdAsync(
            Guid proectId,
            int page,
            string? searchText
        )
        {
            var result = await _sprintApi.GetSprintsAsync(
                proectId,
                page,
                searchText: searchText
            );

            if (result.IsSuccess && result.Response is not null)
                return result.Response;

            if (!string.IsNullOrWhiteSpace(result.Message))
            {
                _globalNotificationService.ShowError(result.Message);
                if (_logger.IsEnabled(LogLevel.Warning))
                    _logger.LogWarning("Get sprints failed: {Error}", result.Message);
            }

            return new ListDataResponse<Sprint>(0, []);
        }

        public async Task<Sprint?> GetActiveSprintByProjectIdAsync(Guid proectId)
        {
            var result = await _sprintApi.GetActiveSprintAsync(proectId);

            if (!string.IsNullOrWhiteSpace(result.Message))
            {
                _globalNotificationService.ShowError(result.Message);
                if (_logger.IsEnabled(LogLevel.Warning))
                    _logger.LogWarning("Get active sprint failed: {Error}", result.Message);
            }

            return result.Response;
        }

        public async Task<bool> CreateSprintAsync(CreateSprintRequest request)
        {
            var result = await _sprintApi.CreateSprintAsync(request);
            if (result.IsSuccess && result.Response is not null)
            {
                OnSprintHasCreated?.Invoke(result.Response);
                _globalNotificationService.ShowSuccess("Спринт успешно создан!");
                return true;
            }

            if (!string.IsNullOrWhiteSpace(result.Message))
            {
                _globalNotificationService.ShowError(result.Message);
                if (_logger.IsEnabled(LogLevel.Warning))
                    _logger.LogWarning("Create sprint failed: {Error}", result.Message);
            }

            return false;
        }

        public async Task<bool> UpdateSprintAsync(Guid sprintId, UpdateSprintRequest request)
        {
            var result = await _sprintApi.UpdateSprintAsync(sprintId, request);
            if (result.IsSuccess && result.Response is not null)
            {
                OnSprintHasUpdated?.Invoke(result.Response);
                _globalNotificationService.ShowSuccess("Спринт успешно изменен!");
                return true;
            }

            if (!string.IsNullOrWhiteSpace(result.Message))
            {
                _globalNotificationService.ShowError(result.Message);
                if (_logger.IsEnabled(LogLevel.Warning))
                    _logger.LogWarning("Update sprint failed: {Error}", result.Message);
            }

            return false;
        }

        public async Task<bool> FinishSprintAsync(Guid sprintId, string report, IEnumerable<SprintMarkRequest> marks)
        {
            var resultFinish = await _sprintApi.FinishSprintAsync(sprintId, report);
            if (resultFinish.IsSuccess && resultFinish.Response is not null)
            {
                var resultMarks = await _sprintApi.CreateSprintMarksAsync(sprintId, marks);
                if (resultMarks.IsSuccess && resultMarks.Response is not null)
                {
                    _globalNotificationService.ShowSuccess(resultMarks.Response);
                }
                else
                {
                    _globalNotificationService.ShowError("Не удалось выставить оценки");
                    if (_logger.IsEnabled(LogLevel.Warning))
                        _logger.LogWarning("Create sprint marks failed: {Error}", resultMarks.Message);
                }

                OnSprintHasFinished?.Invoke();
                _globalNotificationService.ShowSuccess("Спринт успешно завершен!");
                return true;
            }

            if (!string.IsNullOrWhiteSpace(resultFinish.Message))
            {
                _globalNotificationService.ShowError(resultFinish.Message);
                if (_logger.IsEnabled(LogLevel.Warning))
                    _logger.LogWarning("Finish sprint failed: {Error}", resultFinish.Message);
            }

            return false;
        }

        //SprintMarks
        public async Task<List<SprintMarks>> GetSprintMarksBySprintIdAsync(Guid sprintId)
        {
            var result = await _sprintApi.GetSprintMarksAsync(sprintId);
            if (result.IsSuccess && result.Response is not null)
                return result.Response.ToList();

            if (!string.IsNullOrWhiteSpace(result.Message))
            {
                _globalNotificationService.ShowError(result.Message);
                if (_logger.IsEnabled(LogLevel.Warning))
                    _logger.LogWarning("Get sprint marks failed: {Error}", result.Message);
            }

            return [];
        }

        //Tasks
        public async Task<ListDataResponse<HITSTask>> GetTasksByQueryParamsAsync(
            int page,
            Guid? projectId,
            Guid? sprintId,
            string? searchText,
            IEnumerable<HITSTaskStatus>? selectedStatuses,
            IEnumerable<Guid>? selectedTags,
            IEnumerable<Guid>? selectedExecutors
        )
        {
            var result = await _taskApi.GetTasksAsync(
                page,
                projectId: projectId,
                sprintId: sprintId,
                searchText: searchText,
                selectedStatuses: selectedStatuses,
                selectedTags: selectedTags,
                selectedExecutors: selectedExecutors
            );

            if (result.IsSuccess && result.Response is not null)
                return result.Response;

            if (!string.IsNullOrWhiteSpace(result.Message))
            {
                _globalNotificationService.ShowError(result.Message);
                if (_logger.IsEnabled(LogLevel.Warning))
                    _logger.LogWarning("Get tasks failed: {Error}", result.Message);
            }

            return new ListDataResponse<HITSTask>(0, []);
        }

        public async Task<ListDataResponse<TaskMovementLog>> GetTasksLogsAsync(
            Guid taskId,
            int page
        )
        {
            var result = await _taskApi.GetTasksLogsAsync(taskId, page);
            if (result.IsSuccess && result.Response is not null)
                return result.Response;

            if (!string.IsNullOrWhiteSpace(result.Message))
            {
                _globalNotificationService.ShowError(result.Message);
                if (_logger.IsEnabled(LogLevel.Warning))
                    _logger.LogWarning("Get task logs failed: {Error}", result.Message);
            }

            return new ListDataResponse<TaskMovementLog>(0, []);
        }

        public async Task<HITSTask?> GetTaskByIdAsync(Guid taskId)
        {
            var result = await _taskApi.GetTaskAsync(taskId);

            if (!string.IsNullOrWhiteSpace(result.Message))
            {
                _globalNotificationService.ShowError(result.Message);
                if (_logger.IsEnabled(LogLevel.Warning))
                    _logger.LogWarning("Get task failed: {Error}", result.Message);
            }

            return result.Response;
        }

        public async Task<bool> MemberHasTaskInProgressAsync(Guid? sprintId)
        {
            if (!sprintId.HasValue) return false;
            if (_authService.CurrentUser is null) return false;

            var result = await _taskApi.MemberHasActiveTaskAsync(sprintId.Value);
            return result.IsSuccess;
        }

        public async Task<bool> CreateNewTaskAsync(HITSTaskStatus taskStatus, CreateTaskRequest request)
        {
            var result = await _taskApi.CreateTaskAsync(request);
            if (result.IsSuccess && result.Response is not null)
            {
                OnTaskHasCreated?.Invoke(result.Response);
                return true;
            }

            if (!string.IsNullOrWhiteSpace(result.Message))
            {
                _globalNotificationService.ShowError(result.Message);
                if (_logger.IsEnabled(LogLevel.Warning))
                    _logger.LogWarning("Create task failed: {Error}", result.Message);
            }

            return false;
        }

        public async Task<bool> UpdateTaskAsync(Guid taskId, UpdateTaskRequest request)
        {
            var updatedTask = MockSprints.UpdateTask(taskId, request);
            if (updatedTask is null) return false;

            OnTaskHasUpdated?.Invoke(updatedTask);
            return true;
        }

        public async Task<bool> UpdateTaskCommentAsync(Guid taskId, string comment, ProjectMemberRole executorRole)
        {
            var taskHasUpdated = MockSprints.UpdateTaskComment(taskId, comment, executorRole);
            if (!taskHasUpdated) return false;

            OnTaskCommentUpdated?.Invoke(taskId, comment, executorRole);
            return true;
        }

        public async Task<bool> UpdateTaskStatusAsync(HITSTask task, HITSTaskStatus newStatus, int taskIndex)
        {
            var currentUser = _authService.CurrentUser;
            if (currentUser is null) return false;

            var oldStatus = task.Status;

            var updatedTask = MockSprints.UpdateTaskStatus(task.Id, newStatus, currentUser, taskIndex);
            if (updatedTask is null) return false;

            OnTaskHasMoved?.Invoke(updatedTask, oldStatus);
            return true;
        }

        public async Task<bool> UpdateTaskPositionAsync(HITSTask task, int newIndex)
        {
            var taskHasUpdated = MockSprints.UpdateTaskPosition(task.Id, newIndex);
            if (taskHasUpdated is null) return false;

            return true;
        }

        public async Task<bool> UpdateTaskPositionsAsync(ICollection<HITSTask> tasks)
            => MockSprints.UpdateTaskPositions(tasks);

        public async Task<bool> DeleteTaskAsync(HITSTask task)
        {
            if (!MockSprints.DeleteTask(task))
            {
                return false;
            }

            OnTaskHasDeleted?.Invoke(task);
            return true;
        }
    }
}
