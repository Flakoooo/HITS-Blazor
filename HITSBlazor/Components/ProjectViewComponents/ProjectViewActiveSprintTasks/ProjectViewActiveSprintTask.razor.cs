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
        private static event Action? OnDragStateChanged;

        private static double _mouseX;
        private static double _mouseY;

        private bool _isDragOver;
        private bool _isMouseDown;
        private HITSTask? _potentialDragTask;

        private readonly List<HITSTask> _sprintTasks = [];

        private static bool IsDragging => _draggedTask != null;

        protected override async SharpTask OnInitializedAsync()
        {
            _isLoading = true;

            _jsRuntime = JSRuntime;

            ProjectService.OnTaskHasCreated += TaskHasCreated;
            ProjectService.OnTaskHasMoved += TaskHasMoved;
            OnDragStateChanged += HandleDragStateChanged;

            await LoadTasksAsync();

            _isLoading = false;
            MarkAsInitialized();
        }

        protected override async SharpTask AdditionalAfterRenderMethod()
        {
            if (_jsRuntime != null)
                await _jsRuntime.InvokeVoidAsync("dragDrop.initializeGlobalMouseEvents", _dotNetHelper);
        }

        private void HandleDragStateChanged() => InvokeAsync(StateHasChanged);

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

        //TODOO: участник не может перенести идею в На доработке и На проверке
        //TODOO: можно переносить спокойно из Новая в В работе и наоборот
        //TODOO: также нужно сделать так, чтобы расположение задач в колонке можно было менять (с анимацией)
        private bool CanDragTask(HITSTask task)
        {
            var currentUser = AuthService.CurrentUser;
            if (currentUser?.Role is RoleType.Admin) return true;

            if (currentUser?.Id == CurrentMember?.UserId 
                && CurrentMember?.ProjectRole is ProjectMemberRole.TeamLeader or ProjectMemberRole.Initiator) return true;

            if (task.Status is HITSTaskStatus.OnVerification or HITSTaskStatus.OnModification or HITSTaskStatus.Done)
                return false;

            if (task.Executor is null || AuthService.CurrentUser?.Id == task.Executor?.Id)
                return true;

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
                ShowSprintTaskModal(_potentialDragTask.Id);

            _isMouseDown = false;
            _potentialDragTask = null;
        }

        private void HandleMouseMove(MouseEventArgs e)
        {
            if (IsDragging)
            {
                _mouseX = e.ClientX;
                _mouseY = e.ClientY;
                OnDragStateChanged?.Invoke();
            }
            else if (_isMouseDown && _potentialDragTask != null)
            {
                StartDrag(_potentialDragTask);
                _isMouseDown = false;
            }
        }

        [JSInvokable]
        public void OnGlobalMouseMove(double clientX, double clientY)
        {
            if (IsDragging)
            {
                _mouseX = clientX;
                _mouseY = clientY;
                OnDragStateChanged?.Invoke();
            }
        }

        [JSInvokable]
        public void OnGlobalMouseUp(double clientX, double clientY, string? targetCategory)
        {
            if (IsDragging)
            {
                _mouseX = clientX;
                _mouseY = clientY;
                OnDragStateChanged?.Invoke();

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

        private static async void StartDrag(HITSTask task)
        {
            _draggedTask = task;
            _draggedFromCategory = task.Status.ToString();

            if (_jsRuntime != null)
            {
                await _jsRuntime.InvokeVoidAsync("dragDrop.preventSelection");
                await _jsRuntime.InvokeVoidAsync("dragDrop.startGlobalDrag");
            }

            OnDragStateChanged?.Invoke();
        }

        private static async void EndDrag()
        {
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

            EndDrag();

            if (taskToMove != null && taskToMove.Status != TaskCategory)
            {
                try
                {
                    await ProjectService.UpdateTaskStatusAsync(taskToMove, TaskCategory);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error updating task status: {ex.Message}");
                }
            }
        }

        private void TaskHasCreated(HITSTask newTask)
        {
            _sprintTasks.Add(newTask);
            ++_totalCount;
            StateHasChanged();
        }

        private void TaskHasMoved(HITSTask updatedTask, HITSTaskStatus oldStatus)
        {
            if (updatedTask.SprintId != CurrentSprint?.Id) return;

            if (updatedTask.Status == TaskCategory && !_sprintTasks.Any(t => t.Id == updatedTask.Id))
            {
                _sprintTasks.Add(updatedTask);
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

        private string GetTaskExecuteColor(User? executor)
        {
            if (AuthService.CurrentUser is not null
                && executor is not null
                && executor.Id == AuthService.CurrentUser.Id
            ) return "13, 110, 253";

            return "158, 158, 158";
        }

        private void ShowTaskModal(HITSTask? task = null) => ModalService.ShowTaskModal(task);

        private void ShowSprintTaskModal(Guid taskId)
        {
            ModalService.Show<TaskInfoModal>(
                ModalType.RightSide,
                parameters: new Dictionary<string, object> { [nameof(TaskInfoModal.TaskId)] = taskId }
            );
        }

        protected override async ValueTask DisposeAsyncCore()
        {
            ProjectService.OnTaskHasCreated -= TaskHasCreated;
            ProjectService.OnTaskHasMoved -= TaskHasMoved;
            OnDragStateChanged -= HandleDragStateChanged;

            _dotNetHelper?.Dispose();

            if (_jsRuntime != null)
            {
                await _jsRuntime.InvokeVoidAsync("dragDrop.removeGlobalMouseEvents");
            }

            await ValueTask.CompletedTask;
        }
    }
}
