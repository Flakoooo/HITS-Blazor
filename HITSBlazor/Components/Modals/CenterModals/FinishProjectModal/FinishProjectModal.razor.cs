using HITSBlazor.Models.Projects.Entities;
using HITSBlazor.Models.Projects.Enums;
using HITSBlazor.Services;
using HITSBlazor.Services.Modal;
using HITSBlazor.Services.Projects;
using Microsoft.AspNetCore.Components;

using ShrapTask = System.Threading.Tasks.Task;

namespace HITSBlazor.Components.Modals.CenterModals.FinishProjectModal
{
    public partial class FinishProjectModal
    {
        [Inject]
        private IProjectService ProjectService { get; set; } = null!;

        [Inject]
        private GlobalNotificationService NotificationService { get; set; } = null!;

        [Inject]
        private ModalService ModalService { get; set; } = null!;

        [Parameter]
        public required Project CurrentProject { get; set; }

        private bool _isLoading = true;

        private Dictionary<Guid, double> _projectMembersScores = [];

        private string Report { get; set; } = string.Empty;

        private bool _hasActiveSprint = false;

        protected override async ShrapTask OnInitializedAsync()
        {
            _isLoading = true;

            _projectMembersScores = (await ProjectService.GetProjectMarksAsync(CurrentProject.Id))
                .ToDictionary(am => am.UserId, am => am.Mark);

            if (CurrentProject.Status is ProjectStatus.Done)
                Report = CurrentProject.Report ?? string.Empty;
            else
                _hasActiveSprint = await ProjectService.GetActiveSprintByProjectIdAsync(CurrentProject.Id) is not null;

            _isLoading = false;
        }

        private async ShrapTask FinishProject()
        {
            if (_hasActiveSprint) return;

            if (string.IsNullOrWhiteSpace(Report))
            {
                NotificationService.ShowError("Поле отчета не заполнено");
                return;
            }

            if (await ProjectService.FinishProjectAsync(CurrentProject.Id, Report))
                await ModalService.Close(ModalType.Center);
        }
    }
}
