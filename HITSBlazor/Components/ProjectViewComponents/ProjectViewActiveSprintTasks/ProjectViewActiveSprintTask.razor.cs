using HITSBlazor.Models.Projects.Entities;
using HITSBlazor.Models.Users.Entities;
using HITSBlazor.Services.Auth;
using HITSBlazor.Services.Modal;
using HITSBlazor.Services.Projects;
using Microsoft.AspNetCore.Components;

using HITSTask = HITSBlazor.Models.Projects.Entities.Task;
using HITSTaskStatus = HITSBlazor.Models.Projects.Enums.TaskStatus;
using ShrapTask = System.Threading.Tasks.Task;

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
        public Sprint? CurrentSprint { get; set; }

        [Parameter]
        public required HITSTaskStatus TaskCategory { get; set; }

        private bool _isLoading = true;

        private List<HITSTask> _sprintTasks = [];

        protected override async ShrapTask OnInitializedAsync()
        {
            _isLoading = true;

            await LoadTasksAsync();

            _isLoading = false;
            MarkAsInitialized();
        }

        protected override int GetCurrentItemsCount() => _sprintTasks.Count;

        protected override async ShrapTask OnLoadMoreItemsAsync()
        {
            await LoadTasksAsync(append: true);
        }

        private async ShrapTask LoadTasksAsync(bool append = false) => await LoadDataAsync(
            _sprintTasks,
            () => ProjectService.GetTasksByQueryParamsAsync(
                _currentPage, 
                sprintId: CurrentSprint?.Id,
                selectedStatuses: [TaskCategory]
            ),
            append: append
        );

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
    }
}
