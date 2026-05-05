using HITSBlazor.Models.Common.Responses;
using HITSBlazor.Models.Projects.Entities;
using HITSBlazor.Models.Projects.Enums;
using HITSBlazor.Models.Projects.Requests;
using HITSBlazor.Services.Auth;
using HITSBlazor.Utils.Mocks.Projects;

using HITSTask = HITSBlazor.Models.Projects.Entities.Task;
using HITSTaskStatus = HITSBlazor.Models.Projects.Enums.TaskStatus;

namespace HITSBlazor.Services.Projects
{
    public class MockProjectService(IAuthService authService) : IProjectService
    {
        private readonly IAuthService _authService = authService;

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
        ) => MockProjects.GetAllProjects(
            page, searchText: searchText, selectedStatus: selectedStatus
        );

        public async Task<List<Project>> GetAllActiveProjectsAsync()
        {
            var currentUser = _authService.CurrentUser;
            if (currentUser is null) return [];

            return MockProjects.GetActiveProjects(currentUser.Id);
        }

        public async Task<Project?> GetProjectByIdAsync(Guid projectId)
            => MockProjects.GetProjectById(projectId);

        public async Task<ProjectMember?> GetCurrentProjectMemberAsync(Guid projectId)
        {
            var currentUser = _authService.CurrentUser;
            if (currentUser is null) return null;
            
            return MockProjects.GetCurrentProjectMember(projectId, currentUser.Id);
        }

        public async Task<bool> FinishProjectAsync(Guid projectId, string report)
            => MockProjects.FinishProject(projectId, report);

        //ProjectMarks
        public async Task<List<AverageMark>> GetProjectMarksAsync(Guid projectId)
            => MockAverageMarks.GetAverageMarkByProjectId(projectId);

        //Sprints
        public async Task<ListDataResponse<Sprint>> GetSprintsByProjectIdAsync(
            Guid proectId,
            int page
        ) => MockSprints.GetSprintsByProjectId(proectId, page);

        public async Task<Sprint?> GetActiveSprintByProjectIdAsync(Guid proectId)
            => MockSprints.GetActiveSprintByProjectId(proectId);

        public async Task<bool> CreateSprintAsync(Guid projectId, CreateSprintRequest request)
        {
            var newSprint = MockSprints.CreateSprint(projectId, request);
            if (newSprint is null) return false;

            OnSprintHasCreated?.Invoke(newSprint);
            return true;
        }

        public async Task<bool> UpdateSprintAsync(Guid projectId, UpdateSprintRequest request)
        {
            var newSprint = MockSprints.UpdateSprint(projectId, request);
            if (newSprint is null) return false;

            OnSprintHasUpdated?.Invoke(newSprint);
            return true;
        }

        public async Task<bool> FinishSprintAsync(Guid sprintId, IEnumerable<SprintMarkRequest> marks)
        {
            var result = MockSprints.FinishSprint(sprintId, marks);
            if (!result) return false;

            OnSprintHasFinished?.Invoke();
            return true;
        }

        //Tasks
        public async Task<ListDataResponse<HITSTask>> GetTasksByQueryParamsAsync(
            int page,
            Guid? projectId,
            Guid? sprintId,
            IEnumerable<HITSTaskStatus>? selectedStatuses
        ) => MockSprints.GetTasksByQueryParams(
            page, 
            projectId: projectId, 
            sprintId: sprintId, 
            selectedStatuses: selectedStatuses?.ToHashSet()
        );

        public async Task<ListDataResponse<TaskMovementLog>> GetTasksLogsAsync(
            Guid taskId,
            int page
        ) => MockTaskMovementLogs.GetTasksLogsById(taskId, page);

        public async Task<HITSTask?> GetTaskByIdAsync(Guid taskId) => MockSprints.GetTaskById(taskId);

        public async Task<bool> MemberHasTaskInProgressAsync(Guid? sprintId)
        {
            if (!sprintId.HasValue) return false;
            if (_authService.CurrentUser is null) return false;

            return MockSprints.MemberHasTaskInProgress(sprintId.Value, _authService.CurrentUser.Id);
        }

        public async Task<bool> CreateNewTaskAsync(CreateTaskRequest request)
        {
            var newTask = MockSprints.CreateTask(request);
            if (newTask is null) return false;

            OnTaskHasCreated?.Invoke(newTask);
            return true;
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
