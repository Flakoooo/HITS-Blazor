using HITSBlazor.Models.Common.Entities;
using HITSBlazor.Models.Projects.Entities;
using HITSBlazor.Models.Projects.Enums;
using HITSBlazor.Models.Users.Enums;
using HITSBlazor.Services.Auth;
using HITSBlazor.Services.DragAndDrop;
using HITSBlazor.Services.Modal;
using HITSBlazor.Services.Projects;
using HITSBlazor.Services.Tags;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using HITSTask = HITSBlazor.Models.Projects.Entities.Task;
using HITSTaskStatus = HITSBlazor.Models.Projects.Enums.TaskStatus;
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

        [Inject] 
        private DragDropService DragDrop { get; set; } = null!;

        [Parameter]
        public bool IsLoading { get; set; } = false;

        [Parameter]
        public Project? CurrentProject { get; set; }

        private bool IsOnLoading => _isLoading || IsLoading;

        private bool _isLoading = true;

        private readonly List<HITSTask> _projectTasks = [];

        private bool _isMouseDown;
        private HITSTask? _potentialDragTask;
        private bool _renderScheduled;

        private bool IsDragging => DragDrop.IsDragging;

        private HashSet<Tag> SelectedTagNames { get; set; } = [];
        private string SearchTagFilterText { get; set; } = string.Empty;

        protected override async SharpTask OnInitializedAsync()
        {
            _isLoading = true;
            DragDrop.SetJSRuntime(JSRuntime);
            DragDrop.OnDragStateChanged += HandleDragStateChanged;
            DragDrop.OnCleanupTempTask += CleanupTempTask;

            ProjectService.OnTaskHasCreated += TaskHasCreated;
            ProjectService.OnTaskHasUpdated += TaskHasUpdated;
            ProjectService.OnTaskHasDeleted += TaskHasDeleted;

            await LoadTasksAsync();
            _isLoading = false;
            MarkAsInitialized();
        }

        protected override async SharpTask AdditionalAfterRenderMethod()
        {
            await JSRuntime.InvokeVoidAsync("dragDrop.initializeGlobalMouseEvents", _dotNetHelper, HITSTaskStatus.InBackLog.ToString());
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
                var beforeCount = _projectTasks.Count;
                var beforeTasks = _projectTasks.Select(t => t.Id).ToHashSet();

                await LoadDataAsync(
                    _projectTasks,
                    () => ProjectService.GetTasksByQueryParamsAsync(
                        _currentPage, projectId: CurrentProject.Id
                    ),
                    append: append
                );

                if (append && _projectTasks.Count > beforeCount)
                {
                    var sorted = _projectTasks
                        .OrderBy(t => t.Status == HITSTaskStatus.InBackLog ? 0
                                    : t.Status == HITSTaskStatus.Done ? 2 : 1)
                        .ThenBy(t => t.Position)
                        .ToList();
                    _projectTasks.Clear();
                    _projectTasks.AddRange(sorted);
                }
                else if (!append)
                {
                    var sorted = _projectTasks
                        .OrderBy(t => t.Status == HITSTaskStatus.InBackLog ? 0
                                    : t.Status == HITSTaskStatus.Done ? 2 : 1)
                        .ThenBy(t => t.Position)
                        .ToList();
                    _projectTasks.Clear();
                    _projectTasks.AddRange(sorted);
                }
            }
        }

        private void HandleDragStateChanged()
        {
            if (!_renderScheduled)
            {
                _renderScheduled = true;
                InvokeAsync(() => { _renderScheduled = false; StateHasChanged(); });
            }
        }

        private void CleanupTempTask() { }

        private bool CanDragTask(HITSTask task)
        {
            if (task.Status != HITSTaskStatus.InBackLog) return false;

            var currentUser = AuthService.CurrentUser;
            if (currentUser is null) return false;
            if (currentUser.Role is RoleType.Admin) return true;
            if (CurrentProject?.Members.FirstOrDefault(m =>
                    m.UserId == currentUser.Id && m.ProjectRole is ProjectMemberRole.TeamLeader or ProjectMemberRole.Initiator
                )?.UserId == currentUser.Id
            ) return true;

            return task.Executor is null || task.Executor.Id == currentUser.Id;
        }

        private void HandleMouseDown(MouseEventArgs e, HITSTask task)
        {
            if (!CanDragTask(task)) return;
            _isMouseDown = true;
            _potentialDragTask = task;
            DragDrop.UpdateMouseMove(e.ClientX, e.ClientY, null, -1);
        }

        private void HandleMouseUp(MouseEventArgs e)
        {
            if (IsDragging) return;
            if (_isMouseDown && _potentialDragTask is not null)
                ShowTaskModal(_potentialDragTask);
            _isMouseDown = false;
            _potentialDragTask = null;
        }

        private async void HandleMouseMove(MouseEventArgs e)
        {
            if (IsDragging)
            {
                DragDrop.UpdateMouseMove(e.ClientX, e.ClientY, DragDrop.TargetCategory, DragDrop.TargetDropIndex);
            }
            else if (_isMouseDown && _potentialDragTask != null)
            {
                var deltaX = Math.Abs(e.ClientX - DragDrop.MouseX);
                var deltaY = Math.Abs(e.ClientY - DragDrop.MouseY);
                if (deltaX > 5 || deltaY > 5)
                {
                    await DragDrop.StartDrag(_potentialDragTask, HITSTaskStatus.InBackLog.ToString());
                    _isMouseDown = false;
                }
            }
        }

        [JSInvokable]
        public void OnGlobalMouseMove(double clientX, double clientY, string? targetCategory, int dropIndex)
        {
            if (!IsDragging) return;

            DragDrop.UpdateMouseMove(clientX, clientY, targetCategory, dropIndex);

            var task = DragDrop.DraggedTask;
            if (task != null
                && DragDrop.DraggedFromCategory == HITSTaskStatus.InBackLog.ToString()
                && targetCategory == HITSTaskStatus.InBackLog.ToString()
                && dropIndex >= 0)
            {
                MoveTaskToIndex(task, dropIndex);
                DragDrop.NotifyStateChanged();
            }
        }

        [JSInvokable]
        public void OnGlobalMouseUp(double clientX, double clientY, string? targetCategory, int dropIndex)
        {
            if (!IsDragging) { _isMouseDown = false; return; }
            DragDrop.UpdateMouseMove(clientX, clientY, targetCategory, dropIndex);
            if (targetCategory == HITSTaskStatus.InBackLog.ToString())
                InvokeAsync(HandleDrop);
            else
                _ = DragDrop.EndDrag();
        }

        private void MoveTaskToIndex(HITSTask task, int newIndex)
        {
            var maxIndex = _projectTasks.Count(t => t.Status == HITSTaskStatus.InBackLog);
            if (newIndex < 0) newIndex = 0;
            if (newIndex > maxIndex) newIndex = maxIndex;

            var currentIndex = _projectTasks.IndexOf(task);
            if (currentIndex < 0 || currentIndex == newIndex) return;

            _projectTasks.RemoveAt(currentIndex);
            if (currentIndex < newIndex) newIndex--;
            _projectTasks.Insert(newIndex, task);

            int pos = 1;
            foreach (var t in _projectTasks.Where(t => t.Status == HITSTaskStatus.InBackLog))
                t.Position = pos++;

            StateHasChanged();
        }

        private async void HandleDrop()
        {
            var taskToMove = DragDrop.DraggedTask;
            var dropIndex = DragDrop.TargetDropIndex;
            if (taskToMove == null) { await DragDrop.EndDrag(); return; }

            var maxIndex = _projectTasks.Count(t => t.Status == HITSTaskStatus.InBackLog);
            if (dropIndex < 0) dropIndex = 0;
            if (dropIndex > maxIndex) dropIndex = maxIndex;

            if (dropIndex != _projectTasks.IndexOf(taskToMove))
            {
                MoveTaskToIndex(taskToMove, dropIndex);
                await ProjectService.UpdateTaskPositionAsync(taskToMove, taskToMove.Position ?? dropIndex + 1);
            }
            await DragDrop.EndDrag();
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
            var insertIndex = _projectTasks.Count;
            if (newTask.Status == HITSTaskStatus.InBackLog)
            {
                insertIndex = _projectTasks.FindIndex(t => t.Status != HITSTaskStatus.InBackLog);
                if (insertIndex < 0) insertIndex = _projectTasks.Count;
            }
            else if (newTask.Status == HITSTaskStatus.Done)
            {
                insertIndex = _projectTasks.Count;
            }
            else
            {
                insertIndex = _projectTasks.FindIndex(t => t.Status == HITSTaskStatus.Done);
                if (insertIndex < 0) insertIndex = _projectTasks.Count;
            }

            _projectTasks.Insert(insertIndex, newTask);
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
