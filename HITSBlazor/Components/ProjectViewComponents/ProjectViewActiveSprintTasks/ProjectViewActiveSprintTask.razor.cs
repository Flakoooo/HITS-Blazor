using HITSBlazor.Components.Modals.RightSideModals.TaskInfoModal;
using HITSBlazor.Models.Projects.Entities;
using HITSBlazor.Models.Projects.Enums;
using HITSBlazor.Models.Users.Entities;
using HITSBlazor.Models.Users.Enums;
using HITSBlazor.Services.Auth;
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

        [Parameter]
        public required HITSTaskStatus TaskCategory { get; set; }

        [Parameter]
        public Sprint? CurrentSprint { get; set; }

        [Parameter]
        public ProjectMember? CurrentMember { get; set; }

        private static IJSRuntime? _jsRuntime;

        private bool _isLoading = true;

        private static HITSTask? _draggedTask;
        private static string? _draggedFromCategory;
        public static event Action? OnDragStateChanged;
        private static event Action? OnCleanupTempTask;
        private static string? _lastTempCategory;
        private string? _targetCategory;
        private int _targetDropIndex = -1;

        private static double _mouseX;
        private static double _mouseY;

        private bool _isDragOver;
        private bool _isMouseDown;
        private HITSTask? _potentialDragTask;

        private bool _lastIsDragOver;
        private bool _renderScheduled;

        private readonly List<HITSTask> _sprintTasks = [];

        private static bool IsDragging => _draggedTask != null;

        public static HITSTask? DraggedTask => _draggedTask;
        public static double MouseX => _mouseX;
        public static double MouseY => _mouseY;

        protected override async SharpTask OnInitializedAsync()
        {
            _isLoading = true;

            _jsRuntime = JSRuntime;

            ProjectService.OnTaskHasCreated += TaskHasCreated;
            ProjectService.OnTaskHasUpdated += TaskHasUpdated;
            ProjectService.OnTaskCommentUpdated += TaskCommentHasUpdated;
            ProjectService.OnTaskHasMoved += TaskHasMoved;
            OnDragStateChanged += HandleDragStateChanged;
            OnCleanupTempTask += CleanupTempTask;

            await LoadTasksAsync();

            _isLoading = false;
            MarkAsInitialized();
        }
        
        protected override async SharpTask AdditionalAfterRenderMethod()
        {
            if (_jsRuntime != null)
                await _jsRuntime.InvokeVoidAsync("dragDrop.initializeGlobalMouseEvents", _dotNetHelper, TaskCategory.ToString());
        }

        private void HandleDragStateChanged()
        {
            if (!_renderScheduled)
            {
                _renderScheduled = true;
                InvokeAsync(() =>
                {
                    _renderScheduled = false;
                    StateHasChanged();
                });
            }
        }
        private void CleanupTempTask()
        {
            if (_draggedTask != null && _sprintTasks.Any(t => t.Id == _draggedTask.Id)
                && TaskCategory.ToString() != _draggedFromCategory
                && _draggedTask.Status != TaskCategory) // ← не удалять если статус уже правильный
            {
                _sprintTasks.Remove(_draggedTask);
                StateHasChanged();
            }
        }

        protected override int GetCurrentItemsCount() => _sprintTasks.Count;

        protected override async SharpTask OnLoadMoreItemsAsync()
        {
            await LoadTasksAsync(append: true);
        }

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
                return task.Status != HITSTaskStatus.Done;

            if (task.Status is HITSTaskStatus.Done) return false;

            if (task.Status is HITSTaskStatus.OnVerification) return false;

            if (task.Status is HITSTaskStatus.OnModification)
                return task.Executor != null && task.Executor.Id == currentUser.Id;

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

            if (from == HITSTaskStatus.NewTask && to == HITSTaskStatus.InProgress) return true;
            if (from == HITSTaskStatus.InProgress && to == HITSTaskStatus.NewTask) return true;

            if (from == HITSTaskStatus.InProgress && to == HITSTaskStatus.OnVerification) return true;

            if (from == HITSTaskStatus.OnModification && to == HITSTaskStatus.OnVerification) return true;

            return false;
        }

        private void HandleMouseDown(MouseEventArgs e, HITSTask task)
        {
            if (!CanDragTask(task)) return;

            _isMouseDown = true;
            _potentialDragTask = task;
            _mouseX = e.ClientX;
            _mouseY = e.ClientY;
        }

        private void HandleMouseUp(MouseEventArgs e)
        {
            if (IsDragging) return;

            if (_isMouseDown && _potentialDragTask is not null)
            {
                ShowSprintTaskModal(_potentialDragTask.Id);
            }

            _isMouseDown = false;
            _potentialDragTask = null;
        }

        private void HandleMouseMove(MouseEventArgs e)
        {
            if (IsDragging)
            {
                _mouseX = e.ClientX;
                _mouseY = e.ClientY;
            }
            else if (_isMouseDown && _potentialDragTask != null)
            {
                var deltaX = Math.Abs(e.ClientX - _mouseX);
                var deltaY = Math.Abs(e.ClientY - _mouseY);

                if (deltaX > 5 || deltaY > 5)
                {
                    _draggedTask = _potentialDragTask;
                    StartDrag(_potentialDragTask);
                    _isMouseDown = false;
                }
            }
        }

        [JSInvokable]
        public void OnGlobalMouseMove(double clientX, double clientY, string? targetCategory, int dropIndex)
        {
            if (IsDragging)
            {
                _mouseX = clientX;
                _mouseY = clientY;
                _targetCategory = targetCategory;

                bool changed = false;

                if (targetCategory == TaskCategory.ToString())
                {
                    if (_targetDropIndex != dropIndex) changed = true;
                    _targetDropIndex = dropIndex;
                }

                // Чужая колонка — временно добавляем задачу
                if (_draggedTask != null && targetCategory != null
                    && targetCategory == TaskCategory.ToString()
                    && targetCategory != _draggedFromCategory
                    && CanDropTask(_draggedTask))
                {
                    if (_lastTempCategory != null && _lastTempCategory != targetCategory)
                    {
                        OnCleanupTempTask?.Invoke();
                        changed = true;
                    }
                    _lastTempCategory = targetCategory;

                    if (!_sprintTasks.Any(t => t.Id == _draggedTask.Id))
                    {
                        _sprintTasks.Insert(dropIndex >= 0 && dropIndex <= _sprintTasks.Count ? dropIndex : 0, _draggedTask);
                        changed = true;
                    }
                    else if (dropIndex >= 0)
                    {
                        MoveTaskToIndex(_draggedTask, dropIndex);
                        changed = true;
                    }
                }

                // Своя колонка
                if (_draggedFromCategory == TaskCategory.ToString()
                    && targetCategory == TaskCategory.ToString()
                    && dropIndex >= 0
                    && _draggedTask != null)
                {
                    OnCleanupTempTask?.Invoke();
                    MoveTaskToIndex(_draggedTask, dropIndex);
                    changed = true;
                }

                bool newDragOver = targetCategory == TaskCategory.ToString()
                    && _draggedFromCategory != targetCategory
                    && _draggedTask != null
                    && CanDropTask(_draggedTask);

                if (newDragOver != _lastIsDragOver) changed = true;
                _lastIsDragOver = newDragOver;
                _isDragOver = newDragOver;

                if (changed)
                    OnDragStateChanged?.Invoke();
            }
        }

        [JSInvokable]
        public void OnGlobalMouseUp(double clientX, double clientY, string? targetCategory, int dropIndex)
        {
            if (IsDragging)
            {
                _mouseX = clientX;
                _mouseY = clientY;
                _targetDropIndex = dropIndex;
                // OnDragStateChanged?.Invoke(); ← УДАЛИТЕ

                if (!string.IsNullOrEmpty(targetCategory) && targetCategory == TaskCategory.ToString())
                {
                    InvokeAsync(HandleDrop);
                }
                else
                {
                    EndDrag();
                }
            }
            else
            {
                _isMouseDown = false;
                _potentialDragTask = null;
            }
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

        private static async void StartDrag(HITSTask task)
        {
            _draggedFromCategory = task.Status.ToString();

            if (_jsRuntime != null)
            {
                await _jsRuntime.InvokeVoidAsync("dragDrop.preventSelection");
                await _jsRuntime.InvokeVoidAsync("dragDrop.startGlobalDrag", task.Status.ToString());
            }

            OnDragStateChanged?.Invoke();
        }

        private static async void EndDrag()
        {
            OnCleanupTempTask?.Invoke();
            _lastTempCategory = null;

            _draggedTask = null;
            _draggedFromCategory = null;

            if (_jsRuntime != null)
            {
                await _jsRuntime.InvokeVoidAsync("dragDrop.allowSelection");
                await _jsRuntime.InvokeVoidAsync("dragDrop.endGlobalDrag");
            }

            OnDragStateChanged?.Invoke();
        }

        private async void HandleDrop()
        {
            _isDragOver = false;
            var taskToMove = _draggedTask;
            var dropIndex = _targetDropIndex;
            var fromCategory = _draggedFromCategory;

            if (taskToMove == null)
            {
                EndDrag();
                return;
            }

            // Перемещение внутри той же колонки
            if (fromCategory == TaskCategory.ToString())
            {
                if (dropIndex >= 0 && dropIndex != _sprintTasks.IndexOf(taskToMove))
                {
                    MoveTaskToIndex(taskToMove, dropIndex);
                    await ProjectService.UpdateTaskPositionAsync(taskToMove, dropIndex);
                }
                EndDrag();
                return;
            }

            // В другую колонку
            if (!CanDropTask(taskToMove))
            {
                EndDrag();
                return;
            }

            if (TaskCategory == HITSTaskStatus.InProgress)
            {
                var currentUser = AuthService.CurrentUser;
                var isTeamLeaderOrAdmin = currentUser?.Role is RoleType.Admin
                    || (currentUser?.Id == CurrentMember?.UserId
                        && CurrentMember?.ProjectRole is ProjectMemberRole.TeamLeader or ProjectMemberRole.Initiator);

                if (!isTeamLeaderOrAdmin)
                {
                    if (await ProjectService.MemberHasTaskInProgressAsync(CurrentSprint?.Id))
                    {
                        EndDrag();
                        return;
                    }
                }
            }

            // Сохраняем индекс до очистки
            var finalIndex = _sprintTasks.IndexOf(taskToMove);
            if (finalIndex < 0) finalIndex = _targetDropIndex >= 0 ? _targetDropIndex : 0;

            try
            {
                await ProjectService.UpdateTaskStatusAsync(taskToMove, TaskCategory, finalIndex);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }

            EndDrag();
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

        private void ShowTaskModal(HITSTask? task = null) => ModalService.ShowTaskModal(task);

        private void ShowSprintTaskModal(Guid taskId)
        {
            if (CurrentMember is not null)
            {
                ModalService.Show<TaskInfoModal>(
                    ModalType.RightSide,
                    parameters: new Dictionary<string, object>
                    {
                        [nameof(TaskInfoModal.TaskId)] = taskId,
                        [nameof(TaskInfoModal.CurrentProjectMember)] = CurrentMember
                    }
                );
            }
        }

        protected override async ValueTask DisposeAsyncCore()
        {
            ProjectService.OnTaskHasCreated -= TaskHasCreated;
            ProjectService.OnTaskHasUpdated -= TaskHasUpdated;
            ProjectService.OnTaskCommentUpdated -= TaskCommentHasUpdated;
            ProjectService.OnTaskHasMoved -= TaskHasMoved;
            OnDragStateChanged -= HandleDragStateChanged;
            OnCleanupTempTask -= CleanupTempTask;

            _dotNetHelper?.Dispose();

            if (_jsRuntime != null)
            {
                await _jsRuntime.InvokeVoidAsync("dragDrop.removeGlobalMouseEvents");
            }

            await ValueTask.CompletedTask;
        }
    }
}
