using HITSBlazor.Models.Common.Entities;
using HITSBlazor.Models.Projects.Entities;
using HITSBlazor.Services.Auth;
using HITSBlazor.Services.Modal;
using HITSBlazor.Services.Projects;
using HITSBlazor.Services.Skills;
using HITSBlazor.Services.Tags;
using Microsoft.AspNetCore.Components;

using HITSTask = HITSBlazor.Models.Projects.Entities.Task;
using SharpTask = System.Threading.Tasks.Task;

namespace HITSBlazor.Components.ProjectViewComponents.ProjectViewBacklogComponent
{
    public partial class ProjectViewBacklog
    {
        [Inject]
        private IAuthService AuthService { get; set; } = null!;

        [Inject]
        private IProjectService ProjectService { get; set; } = null!;

        [Inject]
        private ITagService TagService { get; set; } = null!;

        [Inject]
        private ModalService ModalService { get; set; } = null!;

        [Parameter]
        public bool IsLoading { get; set; } = false;

        [Parameter]
        public Project? CurrentProject { get; set; }

        private bool IsOnLoading => _isLoading || IsLoading;

        private bool _isLoading = true;

        private readonly List<HITSTask> _projectTasks = [];

        private HashSet<Tag> SelectedTagNames { get; set; } = [];
        private string SearchTagFilterText { get; set; } = string.Empty;

        protected override async SharpTask OnInitializedAsync()
        {
            _isLoading = true;

            ProjectService.OnTaskHasCreated += TaskHasCreated;
            ProjectService.OnTaskHasUpdated += TaskHasUpdated;
            ProjectService.OnTaskHasDeleted += TaskHasDeleted;

            await LoadTasksAsync();

            _isLoading = false;

            MarkAsInitialized();
        }

        protected override int GetCurrentItemsCount() => _projectTasks.Count;

        protected override async SharpTask OnLoadMoreItemsAsync()
        {
            await LoadTasksAsync(append: true);
        }

        private async SharpTask LoadTasksAsync(bool append = false)
        {
            if (CurrentProject is not null)
            {
                await LoadDataAsync(
                    _projectTasks,
                    () => ProjectService.GetTasksByQueryParamsAsync(
                        _currentPage, projectId: CurrentProject.Id
                    ),
                    append: append
                );
            }
        }

        private async SharpTask FiltersHasChanged()
        {
            ResetPagination();
            await LoadTasksAsync();
        }

        private async SharpTask ResetFilters()
        {
            SelectedTagNames.Clear();
            SearchTagFilterText = string.Empty;
            await FiltersHasChanged();
        }

        private void ShowTaskModal(HITSTask? task = null) => ModalService.ShowTaskModal(task);

        private void TaskHasCreated(HITSTask newTask)
        {
            _projectTasks.Add(newTask);
            ++_totalCount;
            StateHasChanged();
        }

        private void TaskHasUpdated(HITSTask updatedTask)
        {
            var taskForUpdate = _projectTasks.FirstOrDefault(t => t.Id == updatedTask.Id);
            if (taskForUpdate is null) return;

            taskForUpdate.Name = updatedTask.Name;
            taskForUpdate.Description = updatedTask.Description;
            taskForUpdate.Tags = updatedTask.Tags;
            taskForUpdate.WorkHour = updatedTask.WorkHour;
            StateHasChanged();
        }

        private void TaskHasDeleted(HITSTask task)
        {
            if (_projectTasks.Remove(task))
            {
                --_totalCount;
                StateHasChanged();
            }
        }

        protected override async ValueTask DisposeAsyncCore()
        {
            ProjectService.OnTaskHasCreated -= TaskHasCreated;
            ProjectService.OnTaskHasUpdated -= TaskHasUpdated;
            ProjectService.OnTaskHasDeleted -= TaskHasDeleted;

            await ValueTask.CompletedTask;
        }
    }
}
