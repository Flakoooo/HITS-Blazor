using HITSBlazor.Components.Modals.CenterModals.FinishProjectModal;
using HITSBlazor.Components.Modals.CenterModals.FinishSprintModal;
using HITSBlazor.Models.Projects.Entities;
using HITSBlazor.Models.Projects.Enums;
using HITSBlazor.Models.Users.Enums;
using HITSBlazor.Services.Auth;
using HITSBlazor.Services.Modal;
using Microsoft.AspNetCore.Components;

namespace HITSBlazor.Components.ProjectViewComponents.ProjectViewActiveSprintComponent
{
    public partial class ProjectViewActiveSprint
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

        private void ShowFinishSprintModal()
        {
            var parameters = new Dictionary<string, object>
            {
                [nameof(FinishSprintModal.ProjectMembers)] = CurrentProject?.Members ?? []
            };

            if (ActiveSprint is null) return;
            parameters.Add(nameof(FinishSprintModal.SprintId), ActiveSprint.Id);

            if (CurrentMember is null)
            {
                var currentUser = AuthService.CurrentUser;
                if (currentUser is null) return;

                if (currentUser.Role is RoleType.Admin)
                {
                    parameters.Add(nameof(FinishSprintModal.CurrentProjectMember), new ProjectMember
                    {
                        TeamId = null,
                        UserId = currentUser.Id,
                        Email = currentUser.Email,
                        FirstName = currentUser.FirstName,
                        LastName = currentUser.LastName,
                        ProjectRole = ProjectMemberRole.TeamLeader,
                        StartDate = DateOnly.MinValue,
                        FinishDate = DateOnly.MaxValue
                    });
                }
            }
            else
            {
                parameters.Add(nameof(FinishSprintModal.CurrentProjectMember), CurrentMember);
            }

            ModalService.Show<FinishSprintModal>(
                ModalType.Center,
                parameters: parameters
            );
        }
    }
}
