using HITSBlazor.Models.Projects.Entities;
using HITSBlazor.Models.Projects.Requests;
using HITSBlazor.Services.Modal;
using HITSBlazor.Services.Projects;
using HITSBlazor.Utils.Validation;
using Microsoft.AspNetCore.Components;
using Newtonsoft.Json.Bson;
using System.Globalization;

using HITSTask = HITSBlazor.Models.Projects.Entities.Task;
using HITSTaskStatus = HITSBlazor.Models.Projects.Enums.TaskStatus;
using SharpTask = System.Threading.Tasks.Task;

namespace HITSBlazor.Components.Modals.CenterModals.SprintModal
{
    public partial class SprintModal
    {
        [Inject]
        private IProjectService ProjectService { get; set; } = null!;

        [Inject]
        private ModalService ModalService { get; set; } = null!;

        [Parameter]
        public Guid ProjectId { get; set; }

        [Parameter]
        public Sprint? CurrentSprint { get; set; } = null!;

        private bool _isLoading = true;

        private string Name { get; set; } = string.Empty;
        private string Goal { get; set; } = string.Empty;
        private string StartDate { get; set; } = string.Empty;
        private string FinishDate { get; set; } = string.Empty;
        private int WorkingHours { get; set; } = 0;

        private readonly List<HITSTask> _allTasksInBacklog = [];
        private HashSet<HITSTask> _sprintTasks = [];

        protected override async SharpTask OnInitializedAsync()
        {
            _isLoading = true;

            ProjectService.OnTaskHasCreated += TaskHasCreated;

            await LoadTasksAsync();

            if (CurrentSprint is not null)
            {
                Name = CurrentSprint.Name;
                Goal = CurrentSprint.Goal;
                StartDate = CurrentSprint.StartDate.ToString("yyyy-MM-dd");
                FinishDate = CurrentSprint.FinishDate.ToString("yyyy-MM-dd");

                _sprintTasks = CurrentSprint.Tasks.ToHashSet();
                WorkingHours = _sprintTasks.Sum(t => t.WorkHour);
            }

            _isLoading = false;
            MarkAsInitialized();
        }

        protected override int GetCurrentItemsCount() => _allTasksInBacklog.Count;

        protected override async SharpTask OnLoadMoreItemsAsync() => await LoadTasksAsync(append: true);

        private async SharpTask LoadTasksAsync(bool append = false) => await LoadDataAsync(
            _allTasksInBacklog,
            () => ProjectService.GetTasksByQueryParamsAsync(
                _currentPage,
                projectId: ProjectId,
                selectedStatuses: [HITSTaskStatus.InBackLog]
            ),
            append: append
        );

        private void SelectTask(HITSTask task)
        {
            _sprintTasks.Add(task);
            WorkingHours = _sprintTasks.Sum(t => t.WorkHour);
        }
        private void UnSelectTask(HITSTask task)
        {
            if (CurrentSprint is not null && task.Status is not HITSTaskStatus.InBackLog) return;

            _sprintTasks.Remove(task);
            WorkingHours = _sprintTasks.Sum(t => t.WorkHour);
        }

        private async SharpTask CreateSprint()
        {
            var startDate = DateValidation.ConvertStringDate<DateOnly>(StartDate);
            var finishDate = DateValidation.ConvertStringDate<DateOnly>(FinishDate);
            if (!startDate.HasValue || !finishDate.HasValue) return;

            if (DateValidation.StartDateValidation(startDate.Value).IsValid)
            {
                if (DateValidation.FinishDateValidation(finishDate.Value, startDate.Value).IsValid)
                {
                    var createRequest = new CreateSprintRequest
                    {
                        ProjectId = ProjectId,
                        Name = Name,
                        Goal = Goal,
                        StartDate = startDate.Value,
                        FinishDate = finishDate.Value,
                        WorkingHours = _sprintTasks.Sum(t => t.WorkHour),
                        Tasks = _sprintTasks.Select(t => t.Id).ToList()
                    };

                    if (await ProjectService.CreateSprintAsync(createRequest))
                        await ModalService.Close(ModalType.Center);
                }
            }
        }

        private async SharpTask UpdateSprint()
        {
            if (CurrentSprint is null) return;

            var startDate = DateValidation.ConvertStringDate<DateOnly>(StartDate);
            var finishDate = DateValidation.ConvertStringDate<DateOnly>(FinishDate);
            if (!startDate.HasValue || !finishDate.HasValue) return;

            if (DateValidation.StartDateValidation(startDate.Value).IsValid)
            {
                if (DateValidation.FinishDateValidation(finishDate.Value, startDate.Value).IsValid)
                {
                    var updateSprint = new UpdateSprintRequest
                    {
                        Name = Name,
                        Goal = Goal,
                        StartDate = startDate!.Value,
                        FinishDate = finishDate!.Value,
                        WorkingHours = _sprintTasks.Sum(t => t.WorkHour),
                        Tasks = _sprintTasks.Select(t => t.Id).ToList()
                    };

                    if (await ProjectService.UpdateSprintAsync(CurrentSprint.Id, updateSprint))
                        await ModalService.Close(ModalType.Center);
                }
            }
        }

        private void TaskHasCreated(HITSTask createdTask)
        {
            _allTasksInBacklog.Add(createdTask);
            ++_totalCount;
            StateHasChanged();
        }

        protected override async ValueTask DisposeAsyncCore()
        {
            ProjectService.OnTaskHasCreated -= TaskHasCreated;

            await ValueTask.CompletedTask;
        }
    }
}
