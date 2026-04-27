using HITSBlazor.Models.Common.Entities;
using HITSBlazor.Models.Projects.Requests;
using HITSBlazor.Services.Auth;
using HITSBlazor.Services.Modal;
using HITSBlazor.Services.Projects;
using HITSBlazor.Services.Tags;
using Microsoft.AspNetCore.Components;

using HITSTask = HITSBlazor.Models.Projects.Entities.Task;
using HITSTaskStatus = HITSBlazor.Models.Projects.Enums.TaskStatus;

namespace HITSBlazor.Components.Modals.CenterModals.TaskModal
{
    public partial class TaskModal
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
        public HITSTask? CurrentTask { get; set; }

        [Parameter]
        public required Guid ProjectId { get; set; }

        [Parameter]
        public Guid? SprintId { get; set; }

        private bool _isLoading = true;

        private string TaskName { get; set; } = string.Empty;
        private string TaskDescription { get; set; } = string.Empty;
        private int? Hours { get; set; }

        private HashSet<Tag> SelectedTags { get; set; } = [];

        protected override async Task OnInitializedAsync()
        {
            _isLoading = true;

            if (CurrentTask is not null)
            {
                TaskName = CurrentTask.Name;
                TaskDescription = CurrentTask.Description;
                Hours = CurrentTask.WorkHour;
                SelectedTags = CurrentTask.Tags.ToHashSet();
            }

            _isLoading = false;
        }

        private bool CheckValidValues()
        {
            if (string.IsNullOrWhiteSpace(TaskName)) return false;
            if (string.IsNullOrWhiteSpace(TaskDescription)) return false;
            if (!Hours.HasValue) return false;

            return true;
        }

        private async Task CreateTask()
        {
            var currentUser = AuthService.CurrentUser;
            if (currentUser is null || !CheckValidValues()) return;

            var request = new CreateTaskRequest
            {
                SprintId = SprintId,
                ProjectId = ProjectId,
                Name = TaskName,
                Description = TaskDescription,
                Initiator = currentUser,
                WorkHour = Hours!.Value,
                StartDate = DateTime.UtcNow,
                Tags = SelectedTags,
                Status = SprintId.HasValue ? HITSTaskStatus.NewTask : HITSTaskStatus.InBackLog
            };

            var result = await ProjectService.CreateNewTaskAsync(request);
            if (result)  await ModalService.Close(ModalType.Center);
        }

        private async Task UpdateTask()
        {
            var currentUser = AuthService.CurrentUser;
            if (currentUser is null || CurrentTask is null || !CheckValidValues()) return;

            var request = new UpdateTaskRequest
            {
                Name = TaskName,
                Description = TaskDescription,
                WorkHour = Hours!.Value,
                Tags = SelectedTags
            };

            var result = await ProjectService.UpdateTaskAsync(CurrentTask.Id, request);
            if (result) await ModalService.Close(ModalType.Center);
        }
    }
}
