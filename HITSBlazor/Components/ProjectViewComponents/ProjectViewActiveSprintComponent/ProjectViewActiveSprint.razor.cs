using HITSBlazor.Components.Modals.CenterModals.FinishSprintModal;
using HITSBlazor.Components.ProjectViewComponents.ProjectViewActiveSprintTasks;
using HITSBlazor.Models.Projects.Entities;
using HITSBlazor.Services.Auth;
using HITSBlazor.Services.Modal;
using HITSBlazor.Services.Projects;
using Microsoft.AspNetCore.Components;

namespace HITSBlazor.Components.ProjectViewComponents.ProjectViewActiveSprintComponent
{
    public partial class ProjectViewActiveSprint : IDisposable
    {
        [Inject]
        private IAuthService AuthService { get; set; } = null!;

        [Inject]
        private ModalService ModalService { get; set; } = null!;

        [Parameter]
        public bool IsLoading { get; set; } = false;

        [Parameter]
        public Project? CurrentProject { get; set; }

        [Parameter]
        public ProjectMember? CurrentMember { get; set; }

        [Parameter]
        public Sprint? ActiveSprint { get; set; }

        protected override void OnInitialized()
        {
            ProjectViewActiveSprintTask.OnDragStateChanged += StateHasChanged;
        }
        private void ShowFinishSprintModal() => ModalService.Show<FinishSprintModal>(
            ModalType.Center,
            parameters: new Dictionary<string, object>
            {
                [nameof(FinishSprintModal.ProjectMembers)] = CurrentProject?.Members ?? []
            }
        );

        public void Dispose()
        {
            ProjectViewActiveSprintTask.OnDragStateChanged -= StateHasChanged;
        }

    }
}
