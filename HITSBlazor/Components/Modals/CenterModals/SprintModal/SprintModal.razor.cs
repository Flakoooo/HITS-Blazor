using HITSBlazor.Models.Projects.Entities;
using HITSBlazor.Services.Modal;
using HITSBlazor.Services.Projects;
using HITSBlazor.Utils.Mocks.Projects;
using Microsoft.AspNetCore.Components;
using System.Globalization;

namespace HITSBlazor.Components.Modals.CenterModals.SprintModal
{
    public partial class SprintModal
    {
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

        private List<Models.Projects.Entities.Task> _allTasksInBacklog = [];
        private List<Models.Projects.Entities.Task> _sprintTasks = [];

        protected override async System.Threading.Tasks.Task OnInitializedAsync()
        {
            _isLoading = true;

            _allTasksInBacklog = [.. MockSprints.GetTasksByProjectId(ProjectId).Where(t => t.Status is Models.Projects.Enums.TaskStatus.InBackLog)];

            if (CurrentSprint is not null)
            {
                Name = CurrentSprint.Name;
                Goal = CurrentSprint.Goal ?? string.Empty;
                StartDate = CurrentSprint.StartDate.ToString("yyyy-MM-dd");
                FinishDate = CurrentSprint.FinishDate.ToString("yyyy-MM-dd");

                _sprintTasks = [.. CurrentSprint.Tasks];
            }

            _isLoading = false;
        }

        private static DateTime? ConvertStringToDate(string date)
        {
            if (string.IsNullOrWhiteSpace(date)) return null;

            return DateTimeOffset.Parse(
                date, CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal
            ).UtcDateTime;
        }
    }
}
