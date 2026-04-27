using HITSBlazor.Models.Projects.Entities;
using HITSBlazor.Models.Projects.Enums;
using HITSBlazor.Models.Projects.Requests;
using HITSBlazor.Services.Modal;
using HITSBlazor.Services.Projects;
using HITSBlazor.Utils.Mocks.Projects;
using Microsoft.AspNetCore.Components;
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

        protected override async SharpTask OnLoadMoreItemsAsync()
        {
            await LoadTasksAsync(append: true);
        }

        private async SharpTask LoadTasksAsync(bool append = false) => await LoadDataAsync(
            _allTasksInBacklog,
            () => ProjectService.GetTasksByProjectIdAsync(
                ProjectId,
                _currentPage,
                selectedStatuses: [HITSTaskStatus.InBackLog]
            ),
            append: append
        );

        private void SelectTask(HITSTask task) => _sprintTasks.Add(task);
        private void UnSelectTask(HITSTask task) => _sprintTasks.Remove(task);

        private static DateTime? ConvertStringToDate(string date)
        {
            if (string.IsNullOrWhiteSpace(date)) return null;

            return DateTimeOffset.Parse(
                date, CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal
            ).UtcDateTime;
        }

        private bool CheckValidValues(out DateTime? startDate, out DateTime? finishDate)
        {
            startDate = ConvertStringToDate(StartDate);
            finishDate = ConvertStringToDate(FinishDate);

            if (string.IsNullOrWhiteSpace(Name)) return false;
            if (!startDate.HasValue) return false;
            if (!finishDate.HasValue) return false;

            return true;
        }

        private async SharpTask CreateSprint()
        {
            if (CheckValidValues(out var startDate, out var finishDate))
            {
                var createRequest = new CreateSprintRequest
                {
                    Name = Name,
                    Goal = Goal,
                    StartDate = startDate!.Value,
                    FinishDate = finishDate!.Value,
                    WorkingHours = _sprintTasks.Sum(t => t.WorkHour),
                    Tasks = _sprintTasks
                };

                var result = await ProjectService.CreateSprintAsync(ProjectId, createRequest);
                if (result)  await ModalService.Close(ModalType.Center);
            }
        }

        private async SharpTask UpdateSprint()
        {
            if (CheckValidValues(out var startDate, out var finishDate))
            {
                var updateSprint = new UpdateSprintRequest
                {
                    Name = Name,
                    Goal = Goal,
                    StartDate = startDate!.Value,
                    FinishDate = finishDate!.Value,
                    WorkingHours = _sprintTasks.Sum(t => t.WorkHour),
                    Tasks = _sprintTasks
                };

                var result = await ProjectService.UpdateSprintAsync(ProjectId, updateSprint);
                if (result)  await ModalService.Close(ModalType.Center);
            }
        }
    }
}
