using HITSBlazor.Components.Button;
using HITSBlazor.Services.Modal;
using HITSBlazor.Services.Projects;
using Microsoft.AspNetCore.Components;

using HITSTask = HITSBlazor.Models.Projects.Entities.Task;
using HITSTaskStatus = HITSBlazor.Models.Projects.Enums.TaskStatus;

namespace HITSBlazor.Components.TaskComponent
{
    public partial class TaskComponent
    {
        [Inject]
        private IProjectService ProjectService { get; set; } = null!;

        [Inject]
        private ModalService ModalService { get; set; } = null!;

        [Parameter]
        public bool IsLoading { get; set; } = false;

        [Parameter]
        public HITSTask? Task { get; set; }

        [Parameter]
        public EventCallback<HITSTask> TaskChanged { get; set; }

        [Parameter]
        public EventCallback OnClickTask { get; set; }

        [Parameter]
        public bool DeleteTaskAllowed { get; set; } = false;

        [Parameter]
        public bool CollapseTaskAllowed { get; set; } = false;

        [Parameter]
        public bool StripTaskName { get; set; } = false;

        [Parameter]
        public bool ShowTaskModalAllowed { get; set; } = false;

        [Parameter]
        public int TaskWidth { get; set; } = 100;

        public (string Hint, string Color) GetHintAndColor()
        {
            if (IsLoading || Task is null) return ("Задачи загружаются", "bg-secondary");

            return Task.Status switch
            {
                HITSTaskStatus.InBackLog => ("Приоритетность задачи", "bg-primary"),
                HITSTaskStatus.Done => ("Задача завершена", "bg-secondary"),
                _ => ("Задача в активном спринте", "bg-warning")
            };
        }

        private async Task ClickTask(Func<Task> toggleMethod)
        {
            if (IsLoading) return;

            if (OnClickTask.HasDelegate)
                await OnClickTask.InvokeAsync();
            else
                await toggleMethod.Invoke();

        }

        private void ShowTaskModal()
        {
            if (ShowTaskModalAllowed && Task is not null)
                ModalService.ShowTaskModal(Task.ProjectId, Task);
        }

        private void ShowConfirmModal()
        {
            if (Task is null) return;

            ModalService.ShowConfirmModal(
                $"Вы действительно хотите удалить \"{Task.Name}\"?",
                () => ProjectService.DeleteTaskAsync(Task),
                confirmButtonVariant: ButtonVariant.Danger,
                confirmButtonText: "Удалить"
            );
        }
    }
}
