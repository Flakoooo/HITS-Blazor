using HITSBlazor.Models.Common.Responses;
using HITSBlazor.Models.Markets.Entities;
using HITSBlazor.Models.Projects.Entities;
using HITSBlazor.Models.Projects.Enums;
using HITSBlazor.Models.Projects.Requests;
using HITSTask = HITSBlazor.Models.Projects.Entities.Task;
using HITSTaskStatus = HITSBlazor.Models.Projects.Enums.TaskStatus;

namespace HITSBlazor.Services.Projects
{
    public interface IProjectService
    {
        event Action<Project>? OnProjectStatusHasChanged;

        event Action<Sprint>? OnSprintHasCreated;
        event Action<Sprint>? OnSprintHasUpdated;
        event Action? OnSprintHasFinished;

        event Action<HITSTask>? OnTaskHasCreated;
        event Action<HITSTask>? OnTaskHasUpdated;
        event Action<Guid, string, ProjectMemberRole>? OnTaskCommentUpdated;
        event Action<HITSTask, HITSTaskStatus>? OnTaskHasMoved;
        event Action<HITSTask>? OnTaskHasDeleted;

        //Projects
        Task<ListDataResponse<Project>> GetProjectsByQueryAsync(
            int page,
            string? searchText = null,
            ProjectStatus? selectedStatus = null
        );
        Task<List<Project>> GetAllActiveProjectsAsync();
        Task<Project?> GetProjectByIdAsync(Guid projectId);
        Task<ListDataResponse<ProjectMember>> GetProjectMembersAsync(
            Guid projectId,
            int page,
            string? searchText
        );
        Task<ProjectMember?> GetCurrentProjectMemberAsync(Guid projectId);
        Task<bool> CreateNewProjectAsync(IdeaMarket ideaMarket);
        //TODOO: найти применение
        Task<bool> AddMemberInProjectAsync(Guid projectId, Guid userId);
        //TODOO: найти применение
        Task<bool> KickMemberFromProjectAsync(Guid projectId, Guid userId);
        Task<bool> ActivateProjectAsync(Project project);
        Task<bool> PauseProjectAsync(Project project);
        Task<bool> FinishProjectAsync(Project project, string report);
        Task<bool> DeletedProjectAsync(Project project);

        //ProjectMarks
        Task<List<AverageMark>> GetProjectMarksAsync(Guid projectId);

        //Sprints
        Task<ListDataResponse<Sprint>> GetSprintsByProjectIdAsync(
            Guid proectId,
            int page,
            string? searchText = null
        );
        Task<Sprint?> GetActiveSprintByProjectIdAsync(Guid proectId);
        Task<bool> CreateSprintAsync(Guid projectId, CreateSprintRequest request);
        Task<bool> UpdateSprintAsync(Guid sprintId, UpdateSprintRequest request);
        Task<bool> FinishSprintAsync(Guid sprintId, string report, IEnumerable<SprintMarkRequest> marks);

        //SprintMarks
        Task<List<SprintMarks>> GetSprintMarksBySprintIdAsync(Guid sprintId);

        //Tasks
        Task<ListDataResponse<HITSTask>> GetTasksByQueryParamsAsync(
            int page,
            Guid? projectId = null,
            Guid? sprintId = null,
            string? searchText = null,
            IEnumerable<HITSTaskStatus>? selectedStatuses = null,
            IEnumerable<Guid>? selectedTags = null,
            IEnumerable<Guid>? selectedExecutors = null
        );
        Task<ListDataResponse<TaskMovementLog>> GetTasksLogsAsync(
            Guid taskId,
            int page
        );
        Task<HITSTask?> GetTaskByIdAsync(Guid taskId);
        Task<bool> MemberHasTaskInProgressAsync(Guid? sprintId);
        Task<bool> CreateNewTaskAsync(CreateTaskRequest request);
        Task<bool> UpdateTaskAsync(Guid taskId, UpdateTaskRequest request);
        Task<bool> UpdateTaskCommentAsync(Guid taskId, string comment, ProjectMemberRole executorRole);
        Task<bool> UpdateTaskStatusAsync(HITSTask task, HITSTaskStatus newStatus, int taskIndex = -100);
        Task<bool> UpdateTaskPositionAsync(HITSTask task, int newIndex);
        Task<bool> UpdateTaskPositionsAsync(ICollection<HITSTask> tasks);
        Task<bool> DeleteTaskAsync(HITSTask task);
    }
}
