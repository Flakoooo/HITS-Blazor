using HITSBlazor.Components.Modals.RightSideModals.TaskInfoModal;
using HITSBlazor.Models.Projects.Entities;
using HITSBlazor.Models.Projects.Enums;
using HITSBlazor.Models.Users.Entities;
using HITSBlazor.Models.Users.Enums;
using HITSBlazor.Services.Auth;
using HITSBlazor.Services.DragAndDrop;
using HITSBlazor.Services.Modal;
using HITSBlazor.Services.Projects;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;

using HITSTask = HITSBlazor.Models.Projects.Entities.Task;
using HITSTaskStatus = HITSBlazor.Models.Projects.Enums.TaskStatus;
using SharpTask = System.Threading.Tasks.Task;

namespace HITSBlazor.Components.ProjectViewComponents.ProjectViewActiveSprintTasks
{
    public partial class ProjectViewActiveSprintTask
    {
        [Inject] 
        private IAuthService AuthService { get; set; } = null!;

        [Inject] 
        private IProjectService ProjectService { get; set; } = null!;

        [Inject] 
        private ModalService ModalService { get; set; } = null!;

        [Inject] 
        private DragDropService DragDrop { get; set; } = null!;

        [Parameter] 
        public required HITSTaskStatus TaskCategory { get; set; }
        [Parameter] 
        public Sprint? CurrentSprint { get; set; }
        [Parameter] 
        public ProjectMember? CurrentMember { get; set; }

        private bool _isLoading = true;
        private bool _isDragOver;
        private bool _isMouseDown;
        private HITSTask? _potentialDragTask;
        private bool _lastIsDragOver;
        private bool _renderScheduled;

        private readonly List<HITSTask> _sprintTasks = [];

        private bool IsDragging => DragDrop.IsDragging;

        protected override async SharpTask OnInitializedAsync()
        {
            _isLoading = true;
            DragDrop.SetJSRuntime(JSRuntime);
            DragDrop.OnDragStateChanged += HandleDragStateChanged;
            DragDrop.OnCleanupTempTask += CleanupTempTask;

            ProjectService.OnTaskHasCreated += TaskHasCreated;
            ProjectService.OnTaskHasUpdated += TaskHasUpdated;
            ProjectService.OnTaskCommentUpdated += TaskCommentHasUpdated;
            ProjectService.OnTaskHasMoved += TaskHasMoved;

            await LoadTasksAsync();
            _isLoading = false;
            MarkAsInitialized();
        }

        protected override async SharpTask AdditionalAfterRenderMethod() => await JSRuntime.InvokeVoidAsync(
            "dragDrop.initializeGlobalMouseEvents",
            _dotNetHelper, TaskCategory.ToString()
        );

        private void HandleDragStateChanged()
        {
            if (!_renderScheduled)
            {
                _renderScheduled = true;
                InvokeAsync(() => { _renderScheduled = false; StateHasChanged(); });
            }
        }

        private void CleanupTempTask() => DragDrop.CleanupTempTask(
            () => _sprintTasks.Any(t => t.Id == DragDrop.DraggedTask?.Id),
            () => { 
                _sprintTasks.Remove(DragDrop.DraggedTask!); 
                StateHasChanged(); 
            },
            TaskCategory.ToString(),
            TaskCategory
        );

        protected override int GetCurrentItemsCount() => _sprintTasks.Count;

        protected override async SharpTask OnLoadMoreItemsAsync() => await LoadTasksAsync(append: true);

        private async SharpTask LoadTasksAsync(bool append = false) => await LoadDataAsync(
            _sprintTasks,
            () => ProjectService.GetTasksByQueryParamsAsync(
                _currentPage,
                sprintId: CurrentSprint?.Id,
                selectedStatuses: [TaskCategory]
            ),
            append: append
        );

        private bool CanDragTask(HITSTask task)
        {
            var currentUser = AuthService.CurrentUser;
            if (currentUser is null) return false;

            if (currentUser.Role is RoleType.Admin) return true;
            if (currentUser.Id == CurrentMember?.UserId
                && CurrentMember?.ProjectRole is ProjectMemberRole.TeamLeader or ProjectMemberRole.Initiator)
                return task.Status is not HITSTaskStatus.Done;

            if (DragDrop.DraggedTask is not null) return true;

            if (task.Status is HITSTaskStatus.Done) return false;

            if (task.Status is HITSTaskStatus.OnVerification) return false;

            if (task.Status is HITSTaskStatus.OnModification)
                return task.Executor is not null && task.Executor.Id == currentUser.Id;

            if (task.Executor is null || task.Executor.Id == currentUser.Id)
                return true;

            return false;
        }

        private bool CanDropTask(HITSTask task)
        {
            var currentUser = AuthService.CurrentUser;
            if (currentUser is null) return false;
            if (task.Status == TaskCategory) return false;

            if (currentUser.Role is RoleType.Admin) return true;

            if (currentUser.Id == CurrentMember?.UserId
                && CurrentMember?.ProjectRole is ProjectMemberRole.TeamLeader or ProjectMemberRole.Initiator)
                return true;

            var from = task.Status;
            var to = TaskCategory;

            if (from is HITSTaskStatus.NewTask && to is HITSTaskStatus.InProgress) return true;
            if (from is HITSTaskStatus.InProgress && to is HITSTaskStatus.NewTask) return true;
            if (from is HITSTaskStatus.InProgress && to is HITSTaskStatus.OnVerification) return true;
            if (from is HITSTaskStatus.OnModification && to is HITSTaskStatus.OnVerification) return true;

            return false;
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
                ShowSprintTaskModal(_potentialDragTask.Id);
            _isMouseDown = false;
            _potentialDragTask = null;
        }

        private async void HandleMouseMove(MouseEventArgs e)
        {
            if (IsDragging)
            {
                DragDrop.UpdateMouseMove(e.ClientX, e.ClientY, DragDrop.TargetCategory, DragDrop.TargetDropIndex);
            }
            else if (_isMouseDown && _potentialDragTask is not null)
            {
                var deltaX = Math.Abs(e.ClientX - DragDrop.MouseX);
                var deltaY = Math.Abs(e.ClientY - DragDrop.MouseY);
                if (deltaX > 5 || deltaY > 5)
                {
                    await DragDrop.StartDrag(_potentialDragTask, TaskCategory.ToString());
                    _isMouseDown = false;
                }
            }
        }

        [JSInvokable]
        public void OnGlobalMouseMove(double clientX, double clientY, string? targetCategory, int dropIndex)
        {
            if (!IsDragging) return;

            DragDrop.UpdateMouseMove(clientX, clientY, targetCategory, dropIndex);
            DragDrop.UpdateOverlayIfNeeded();
            bool changed = false;

            if (targetCategory == TaskCategory.ToString() && DragDrop.TargetDropIndex != dropIndex)
                changed = true;

            var task = DragDrop.DraggedTask;
            if (task is not null && targetCategory is not null
                && targetCategory == TaskCategory.ToString()
                && targetCategory != DragDrop.DraggedFromCategory
                && CanDropTask(task))
            {
                if (DragDrop.LastTempCategory is not null && DragDrop.LastTempCategory != targetCategory)
                {
                    DragDrop.NotifyCleanUp();
                    changed = true;
                }
                DragDrop.LastTempCategory = targetCategory;

                if (!_sprintTasks.Any(t => t.Id == task.Id))
                {
                    _sprintTasks.Insert(dropIndex >= 0 && dropIndex <= _sprintTasks.Count ? dropIndex : 0, task);
                    changed = true;
                }
                else if (dropIndex >= 0)
                {
                    MoveTaskToIndex(task, dropIndex);
                    changed = true;
                }
            }

            if (DragDrop.DraggedFromCategory == TaskCategory.ToString()
                && targetCategory == TaskCategory.ToString()
                && dropIndex >= 0 && task is not null)
            {
                DragDrop.NotifyCleanUp();
                MoveTaskToIndex(task, dropIndex);
                changed = true;
            }

            bool newDragOver = targetCategory == TaskCategory.ToString()
                && DragDrop.DraggedFromCategory != targetCategory
                && task != null && CanDropTask(task);

            if (newDragOver != _lastIsDragOver) changed = true;
            _lastIsDragOver = newDragOver;
            _isDragOver = newDragOver;

            if (changed) DragDrop.NotifyStateChanged();
        }

        [JSInvokable]
        public void OnGlobalMouseUp(double clientX, double clientY, string? targetCategory, int dropIndex)
        {
            if (!IsDragging) 
            { 
                _isMouseDown = false; 
                _potentialDragTask = null; 
                return; 
            }

            DragDrop.UpdateMouseMove(clientX, clientY, targetCategory, dropIndex);
            if (!string.IsNullOrEmpty(targetCategory) && targetCategory == TaskCategory.ToString())
                InvokeAsync(HandleDrop);
            else
                _ = DragDrop.EndDrag();
        }

        private void MoveTaskToIndex(HITSTask task, int newIndex)
        {
            if (newIndex < 0 || newIndex > _sprintTasks.Count) return;

            var currentIndex = _sprintTasks.IndexOf(task);
            if (currentIndex < 0 || currentIndex == newIndex) return;

            
            _sprintTasks.RemoveAt(currentIndex);
            if (currentIndex < newIndex) newIndex--;
            _sprintTasks.Insert(newIndex, task);
            StateHasChanged();
        }

        private async void HandleDrop()
        {
            _isDragOver = false;
            var taskToMove = DragDrop.DraggedTask;
            var dropIndex = DragDrop.TargetDropIndex;
            var fromCategory = DragDrop.DraggedFromCategory;

            if (taskToMove is null) 
            { 
                await DragDrop.EndDrag(); 
                return; 
            }

            if (fromCategory == TaskCategory.ToString())
            {
                if (dropIndex >= 0 && dropIndex != _sprintTasks.IndexOf(taskToMove))
                {
                    MoveTaskToIndex(taskToMove, dropIndex);
                }
                await DragDrop.EndDrag();
                return;
            }

            if (!CanDropTask(taskToMove)) 
            { 
                await DragDrop.EndDrag(); 
                return; 
            }

            if (TaskCategory == HITSTaskStatus.InProgress)
            {
                var currentUser = AuthService.CurrentUser;
                var isTeamLeaderOrAdmin = currentUser?.Role is RoleType.Admin
                    || (currentUser?.Id == CurrentMember?.UserId
                        && CurrentMember?.ProjectRole is ProjectMemberRole.TeamLeader or ProjectMemberRole.Initiator);
                if (!isTeamLeaderOrAdmin && await ProjectService.MemberHasTaskInProgressAsync(CurrentSprint?.Id))
                {
                    await DragDrop.EndDrag();
                    return;
                }
            }

            var finalIndex = _sprintTasks.IndexOf(taskToMove);
            if (finalIndex < 0) finalIndex = dropIndex >= 0 ? dropIndex : 0;

            await DragDrop.EndDrag();

            try 
            { 
                await ProjectService.UpdateTaskStatusAsync(taskToMove, TaskCategory, finalIndex); 
            }
            catch (Exception ex) 
            { 
                Console.WriteLine($"Error: {ex.Message}"); 
            }
        }

        private void TaskHasCreated(HITSTask newTask)
        {
            _sprintTasks.Add(newTask);
            ++_totalCount;
            StateHasChanged();
        }

        private void TaskHasUpdated(HITSTask updatedTask)
        {
            if (TaskCategory != updatedTask.Status) return;

            var taskForUpdate = _sprintTasks.FirstOrDefault(t => t.Id == updatedTask.Id);
            if (taskForUpdate is null) return;

            taskForUpdate.Name = updatedTask.Name;
            taskForUpdate.Description = updatedTask.Description;
            taskForUpdate.Tags = updatedTask.Tags;
            StateHasChanged();
        }

        private void TaskCommentHasUpdated(Guid taskId, string comment, ProjectMemberRole executorRole)
        {
            var taskForUpdate = _sprintTasks.FirstOrDefault(t => t.Id == taskId);
            if (taskForUpdate is null) return;

            if (executorRole is ProjectMemberRole.TeamLeader)
                taskForUpdate.LeaderComment = comment;
            else if (executorRole is ProjectMemberRole.Member)
                taskForUpdate.ExecutorComment = comment;
            else
                return;

            StateHasChanged();
        }

        private void TaskHasMoved(HITSTask updatedTask, HITSTaskStatus oldStatus)
        {
            if (updatedTask.SprintId != CurrentSprint?.Id) return;

            if (updatedTask.Status == TaskCategory && !_sprintTasks.Any(t => t.Id == updatedTask.Id))
            {
                var pos = updatedTask.Position ?? _sprintTasks.Count;
                if (pos >= _sprintTasks.Count)
                    _sprintTasks.Add(updatedTask);
                else
                    _sprintTasks.Insert(pos, updatedTask);
                ++_totalCount;

                for (int i = 0; i < _sprintTasks.Count; i++)
                    _sprintTasks[i].Position = i;

                StateHasChanged();
            }
            else if (oldStatus == TaskCategory)
            {
                var taskToRemove = _sprintTasks.FirstOrDefault(t => t.Id == updatedTask.Id);
                if (taskToRemove is not null && _sprintTasks.Remove(taskToRemove))
                {
                    --_totalCount;
                    StateHasChanged();
                }
            }
        }

        private static string GetHintTaskStatusText(HITSTaskStatus status) => status switch
        {
            HITSTaskStatus.OnModification => "Здесь находятся задачи, которые были отправлены на доработку для исправления ошибок или улучшения качества."
                + " Эти задачи нужно выполнить в первую очередь, чтобы не затягивать сроки проекта.",
            HITSTaskStatus.NewTask => "Здесь находятся задачи, которые еще не были назначены разработчику."
                + " Эти задачи можно выбирать по своему усмотрению, учитывая приоритеты и сложность.",
            HITSTaskStatus.InProgress => "Здесь находятся задачи, которые в данный момент выполняются командой или отдельным разработчиком."
                + " Данные задачи нужно довести до конца и не переключаться на другие.",
            HITSTaskStatus.OnVerification => "Здесь находятся задачи, которые были выполнены и отправлены тимлиду на проверку качества, функциональности и требованиям.",
            HITSTaskStatus.Done => "Здесь находятся задачи, которые были успешно проверены и одобрены."
                + " Этизадачи можно считать завершенными и не требующими дальнейшего внимания.",
            _ => $"{nameof(status)} hint text"
        };

        private static string GetTaskCategoryColor(HITSTaskStatus taskCategory) => taskCategory switch
        {
            HITSTaskStatus.NewTask => "#0d6efd",
            HITSTaskStatus.InProgress => "#f5ec0a",
            HITSTaskStatus.OnVerification => "#ffa800",
            HITSTaskStatus.OnModification => "#8a2be2",
            HITSTaskStatus.Done => "#13c63a",
            _ => string.Empty
        };

        private void ShowTaskModal(HITSTask? task = null)
        {
            if (CurrentSprint is null) return;

            ModalService.ShowTaskModal(CurrentSprint.ProjectId, task);
        }

        private void ShowSprintTaskModal(Guid taskId)
        {
            if (CurrentMember is null) return;

            ModalService.Show<TaskInfoModal>(
                ModalType.RightSide,
                parameters: new Dictionary<string, object>
                {
                    [nameof(TaskInfoModal.TaskId)] = taskId,
                    [nameof(TaskInfoModal.CurrentProjectMember)] = CurrentMember
                }
            );
        }

        protected override async ValueTask DisposeAsyncCore()
        {
            DragDrop.OnDragStateChanged -= HandleDragStateChanged;
            DragDrop.OnCleanupTempTask -= CleanupTempTask;

            ProjectService.OnTaskHasCreated -= TaskHasCreated;
            ProjectService.OnTaskHasUpdated -= TaskHasUpdated;
            ProjectService.OnTaskCommentUpdated -= TaskCommentHasUpdated;
            ProjectService.OnTaskHasMoved -= TaskHasMoved;

            try 
            { 
                await JSRuntime.InvokeVoidAsync("dragDrop.removeGlobalMouseEvents"); 
            }
            catch { }

            await ValueTask.CompletedTask;
        }
    }
}
