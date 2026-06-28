using HITSBlazor.Models.Common.Entities;
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
    public class MockProjectService(IAuthService authService) : IProjectService
    {
        private readonly IAuthService _authService = authService;

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

        public async Task<bool> CreateNewProjectAsync(IdeaMarket ideaMarket) => MockProjects.CreateNewProject(ideaMarket);

        //TODOO: реализовать, но как?
        //public async Task<bool> ChangeTeamInProjectAsync()
        //{

        //}

        public async Task<bool> AddMemberInProjectAsync(Guid projectId, Guid userId)
            => MockProjects.AddMemberInProject(projectId, userId);

        public async Task<bool> KickMemberFromProjectAsync(Guid projectId, ProjectMember member)
        {
            if (MockProjects.KickMemberFromProject(projectId, member.UserId))
            {
                OnMemberHasKicked?.Invoke(member);
                return true;
            }

            return false;
        }

        public async Task<bool> ActivateProjectAsync(Project project)
        {
            var updatedProject = MockProjects.UpdateProjectStatus(project.Id, ProjectStatus.Active);
            if (updatedProject is null) return false;


            OnProjectStatusHasChanged?.Invoke(updatedProject);
            return true;
        }

        public async Task<bool> PauseProjectAsync(Project project)
        {
            var updatedProject = MockProjects.UpdateProjectStatus(project.Id, ProjectStatus.Paused);
            if (updatedProject is null) return false;


            OnProjectStatusHasChanged?.Invoke(updatedProject);
            return true;
        }

        public async Task<bool> FinishProjectAsync(Project project, string report)
        {
            var updatedProject = MockProjects.FinishProject(project.Id, report);
            if (updatedProject is null) return false;

            OnProjectStatusHasChanged?.Invoke(updatedProject);
            return true;
        }

        public async Task<bool> DeletedProjectAsync(Project project)
        {
            var updatedProject = MockProjects.DeleteProject(project.Id);
            if (updatedProject is null) return false;

            OnProjectStatusHasChanged?.Invoke(updatedProject);
            return true;
        }

        //ProjectMarks
        public async Task<List<AverageMark>> GetProjectMarksAsync(Guid projectId)
            => MockAverageMarks.GetAverageMarkByProjectId(projectId);

        //Sprints
        public async Task<ListDataResponse<Sprint>> GetSprintsByProjectIdAsync(
            Guid proectId,
            int page,
            string? searchText
        ) => MockSprints.GetSprintsByProjectId(proectId, page, searchText: searchText);

        public async Task<Sprint?> GetActiveSprintByProjectIdAsync(Guid proectId)
            => MockSprints.GetActiveSprintByProjectId(proectId);

        public async Task<bool> CreateSprintAsync(CreateSprintRequest request)
        {
            var newSprint = MockSprints.CreateSprint(request);
            if (newSprint is null) return false;

            OnSprintHasCreated?.Invoke(newSprint);
            return true;
        }

        public async Task<bool> UpdateSprintAsync(Guid sprintId, UpdateSprintRequest request)
        {
            var currentUser = _authService.CurrentUser;
            if (currentUser is null) return false;


            var newSprint = MockSprints.UpdateSprint(sprintId, currentUser, request);
            if (newSprint is null) return false;

            OnSprintHasUpdated?.Invoke(newSprint);
            return true;
        }

        public async Task<bool> FinishSprintAsync(Guid sprintId, string report, IEnumerable<SprintMarkRequest> marks)
        {
            var result = MockSprints.FinishSprint(sprintId, report, marks);
            if (!result) return false;

            OnSprintHasFinished?.Invoke();
            return true;
        }

        //SprintMarks
        public async Task<List<SprintMarks>> GetSprintMarksBySprintIdAsync(Guid sprintId)
            => MockSprintMarks.GetSprintMarksBySprintId(sprintId);

        //Tasks
        public async Task<ListDataResponse<HITSTask>> GetTasksByQueryParamsAsync(
            int page,
            Guid? projectId,
            Guid? sprintId,
            string? searchText,
            IEnumerable<HITSTaskStatus>? selectedStatuses,
            IEnumerable<Guid>? selectedTags,
            IEnumerable<Guid>? selectedExecutors
        ) => MockSprints.GetTasksByQueryParams(
            page,
            projectId: projectId,
            sprintId: sprintId,
            searchText: searchText,
            selectedStatuses: selectedStatuses?.ToHashSet(),
            selectedTags: selectedTags?.ToHashSet(),
            selectedExecutors: selectedExecutors?.ToHashSet()
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

        public async Task<bool> CreateNewTaskAsync(HITSTaskStatus taskStatus, CreateTaskRequest request)
        {
            var currentUser = _authService.CurrentUser;
            if (currentUser is null) return false;

            var newTask = MockSprints.CreateTask(currentUser, taskStatus, request);
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
