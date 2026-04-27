using HITSBlazor.Models.Common.Responses;
using HITSBlazor.Models.Projects.Entities;
using HITSBlazor.Models.Projects.Enums;
using HITSBlazor.Models.Projects.Requests;
using HITSBlazor.Models.Users.Entities;
using HITSBlazor.Utils.Mocks.Projects;

using HITSTask = HITSBlazor.Models.Projects.Entities.Task;
using HITSTaskStatus = HITSBlazor.Models.Projects.Enums.TaskStatus;

namespace HITSBlazor.Services.Projects
{
    public class MockProjectService : IProjectService
    {
        public event Action<Sprint>? OnSprintHasCreated;
        public event Action<Sprint>? OnSprintHasUpdated;

        public event Action<HITSTask>? OnTaskHasCreated;
        public event Action<HITSTask>? OnTaskHasUpdated;
        public event Action<HITSTask>? OnTaskHasDeleted;

        public async Task<ListDataResponse<Project>> GetProjectsByQueryAsync(
            int page,
            string? searchText,
            ProjectStatus? selectedStatus
        ) => MockProjects.GetAllProjects(
            page, searchText: searchText, selectedStatus: selectedStatus
        );

        public async Task<List<Project>> GetAllActiveProjectsAsync(Guid userId)
            => MockProjects.GetActiveProjects(userId);

        public async Task<Project?> GetProjectByIdAsync(Guid projectId)
            => MockProjects.GetProjectById(projectId);

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

        //Tasks
        public async Task<ListDataResponse<HITSTask>> GetTasksByProjectIdAsync(
            Guid proectId,
            int page,
            IEnumerable<HITSTaskStatus>? selectedStatuses
        ) => MockSprints.GetTasksByProjectId(
            proectId, page, selectedStatuses: selectedStatuses?.ToHashSet()
        );

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

        public async Task<bool> UpdateTaskStatusAsync(Guid taskId, HITSTaskStatus newStatus, User executor)
        {
            var updatedTask = MockSprints.UpdateTaskStatus(taskId, newStatus, executor);
            if (updatedTask is null) return false;

            OnTaskHasUpdated?.Invoke(updatedTask);
            return true;
        }

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
