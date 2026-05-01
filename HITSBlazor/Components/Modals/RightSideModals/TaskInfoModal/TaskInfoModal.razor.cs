using HITSBlazor.Components.Button;
using HITSBlazor.Models.Common.Entities;
using HITSBlazor.Models.Projects.Entities;
using HITSBlazor.Models.Projects.Enums;
using HITSBlazor.Models.Projects.Requests;
using HITSBlazor.Models.Users.Enums;
using HITSBlazor.Services.Auth;
using HITSBlazor.Services.Modal;
using HITSBlazor.Services.Projects;
using Microsoft.AspNetCore.Components;

using SharpTask = System.Threading.Tasks.Task;
using HITSTask = HITSBlazor.Models.Projects.Entities.Task;
using HITSTaskStatus = HITSBlazor.Models.Projects.Enums.TaskStatus;

namespace HITSBlazor.Components.Modals.RightSideModals.TaskInfoModal
{
    public partial class TaskInfoModal
    {
        [Inject]
        private IAuthService AuthService { get; set; } = null!;

        [Inject]
        private IProjectService ProjectService { get; set; } = null!;

        [Inject]
        private ModalService ModalService { get; set; } = null!;

        [Parameter]
        public Guid TaskId { get; set; }

        [Parameter]
        public required ProjectMember CurrentProjectMember { get; set; }

        private bool _isLoading = true;

        private HITSTask? _currentTask;

        private bool _isEditorMode = false;

        private string Name { get; set; } = string.Empty;
        private string Description { get; set; } = string.Empty;

        private HashSet<Tag> SelectedTags { get; set; } = [];

        private string TeamLeadComment { get; set; } = string.Empty;
        private string ExecutorComment { get; set; } = string.Empty;

        protected override async SharpTask OnInitializedAsync()
        {
            _isLoading = true;

            _currentTask = await ProjectService.GetTaskByIdAsync(TaskId);
            if (_currentTask is null) return;

            Name = _currentTask.Name;
            Description = _currentTask.Description;
            SelectedTags = _currentTask.Tags.ToHashSet();
            TeamLeadComment = _currentTask.LeaderComment ?? string.Empty;
            ExecutorComment = _currentTask.ExecutorComment ?? string.Empty;

            _isLoading = false;
        }

        //TODOO: нужно как то это разделить на изменение коммента тимлида и исполнителя
        private async SharpTask HandleTextSubmit(string value)
        {
            if (_currentTask is not null && CurrentProjectMember is not null)
                await ProjectService.UpdateTaskCommentAsync(
                    _currentTask.Id, value, CurrentProjectMember.ProjectRole
                );
        }

        private bool EditIsAllowed()
        {
            var currentUser = AuthService.CurrentUser;
            if (currentUser?.Role is RoleType.Admin) return true;

            var currentRole = CurrentProjectMember.ProjectRole;
            if (currentUser?.Id == CurrentProjectMember.UserId
                && CurrentProjectMember.ProjectRole is ProjectMemberRole.TeamLeader or ProjectMemberRole.Initiator)
                return true;

            if (currentUser?.Id == _currentTask?.Executor?.Id)
                return true;

            return false;
        }

        private void StartEditTask()
        {
            _isEditorMode = true;
            StateHasChanged();
        }

        private void EndEditTask()
        {
            if (_currentTask is null) return;

            _isEditorMode = false;

            Name = _currentTask.Name;
            Description = _currentTask.Description;
            SelectedTags = _currentTask.Tags.ToHashSet();
            TeamLeadComment = _currentTask.LeaderComment ?? string.Empty;
            ExecutorComment = _currentTask.ExecutorComment ?? string.Empty;

            StateHasChanged();
        }

        //TODOO: валидацию походу надо
        private async SharpTask ConfirmTaskUpdate()
        {
            if (_currentTask is null) return;

            var request = new UpdateTaskRequest
            {
                Name = Name,
                Description = Description,
                Tags = SelectedTags,
                WorkHour = _currentTask.WorkHour
            };

            var result = await ProjectService.UpdateTaskAsync(_currentTask.Id, request);
            if (!result) return;

            _currentTask.Name = request.Name;
            _currentTask.Description = request.Description;
            _currentTask.Tags = request.Tags.ToList();

            _isEditorMode = false;
            StateHasChanged();
        }

        private void DeleteTask()
        {
            if (_currentTask is null) return;

            ModalService.ShowConfirmModal(
                $"Вы действительно хотите удалить \"{_currentTask.Name}\"?",
                () => ProjectService.DeleteTaskAsync(_currentTask),
                confirmButtonVariant: ButtonVariant.Danger,
                confirmButtonText: "Удалить"
            );
        }
    }
}
