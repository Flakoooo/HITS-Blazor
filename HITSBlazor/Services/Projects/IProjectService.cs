using HITSBlazor.Models.Common.Responses;
using HITSBlazor.Models.Projects.Entities;
using HITSBlazor.Models.Projects.Enums;
using HITSBlazor.Models.Projects.Requests;
using HITSBlazor.Models.Users.Entities;
using HITSTask = HITSBlazor.Models.Projects.Entities.Task;
using HITSTaskStatus = HITSBlazor.Models.Projects.Enums.TaskStatus;

namespace HITSBlazor.Services.Projects
{
    public interface IProjectService
    {
        event Action<Sprint>? OnSprintHasCreated;
        event Action<Sprint>? OnSprintHasUpdated;

        event Action<HITSTask>? OnTaskHasCreated;
        event Action<HITSTask>? OnTaskHasUpdated;
        event Action<HITSTask, HITSTaskStatus>? OnTaskHasMoved;
        event Action<HITSTask>? OnTaskHasDeleted;

        //Projects
        Task<ListDataResponse<Project>> GetProjectsByQueryAsync(
            int page,
            string? searchText = null,
            ProjectStatus? selectedStatus = null
        );
        Task<List<Project>> GetAllActiveProjectsAsync(Guid userId);
        Task<Project?> GetProjectByIdAsync(Guid projectId);

        //Sprints
        Task<ListDataResponse<Sprint>> GetSprintsByProjectIdAsync(
            Guid proectId,
            int page
        );
        Task<Sprint?> GetActiveSprintByProjectIdAsync(Guid proectId);
        Task<bool> CreateSprintAsync(Guid projectId, CreateSprintRequest request);
        Task<bool> UpdateSprintAsync(Guid projectId, UpdateSprintRequest request);

        //Tasks
        Task<ListDataResponse<HITSTask>> GetTasksByQueryParamsAsync(
            int page,
            Guid? projectId = null,
            Guid? sprintId = null,
            IEnumerable<HITSTaskStatus>? selectedStatuses = null
        );
        Task<bool> CreateNewTaskAsync(CreateTaskRequest request);
        Task<bool> UpdateTaskAsync(Guid taskId, UpdateTaskRequest request);
        Task<bool> UpdateTaskStatusAsync(HITSTask task, HITSTaskStatus newStatus, User executor);
        Task<bool> DeleteTaskAsync(HITSTask task);
    }
}
