using HITSBlazor.Models.Projects.Entities;
using HITSBlazor.Models.Projects.Requests;
using HITSBlazor.Services;
using HITSBlazor.Services.Modal;
using HITSBlazor.Services.Projects;
using Microsoft.AspNetCore.Components;

using SharpTask = System.Threading.Tasks.Task;

namespace HITSBlazor.Components.Modals.CenterModals.FinishSprintModal
{
    public partial class FinishSprintModal
    {
        [Inject]
        private IProjectService ProjectService { get; set; } = null!;

        [Inject]
        private GlobalNotificationService NotificationService { get; set; } = null!;

        [Inject]
        private ModalService ModalService { get; set; } = null!;

        [Parameter]
        public Guid SprintId { get; set; }

        [Parameter]
        public List<ProjectMember> ProjectMembers { get; set; } = [];

        private bool _isLoading = true;

        private Dictionary<Guid, int?> _projectMembersScores = [];

        protected override async SharpTask OnInitializedAsync()
        {
            _isLoading = true;

            _projectMembersScores = ProjectMembers.ToDictionary(m => m.UserId, m => (int?)null);

            _isLoading = false;
        }

        private async SharpTask FinishSprint()
        {
            if (_projectMembersScores.Any(t => !t.Value.HasValue))
            {
                NotificationService.ShowError("Выставлены не все оценки");
                return;
            }

            var marks = new List<SprintMarkRequest>();

            foreach (var pair in _projectMembersScores)
            {
                marks.Add(new SprintMarkRequest
                {
                    SprintId = SprintId,
                    UserId = pair.Key,
                    Mark = pair.Value ?? 0
                });
            }

            if (await ProjectService.FinishSprintAsync(SprintId, marks))
                await ModalService.Close(ModalType.Center);
        }
    }
}
