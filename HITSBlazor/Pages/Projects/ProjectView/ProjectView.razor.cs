using HITSBlazor.Components.ProjectViewComponents.ProjectViewActiveSprintTasks;
using HITSBlazor.Models.Projects.Entities;
using HITSBlazor.Models.Projects.Enums;
using HITSBlazor.Services.Auth;
using HITSBlazor.Services.DragAndDrop;
using HITSBlazor.Services.Projects;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;

using HITSTask = HITSBlazor.Models.Projects.Entities.Task;
using SharpTask = System.Threading.Tasks.Task;

namespace HITSBlazor.Pages.Projects.ProjectView
{
    [Authorize]
    [Route("projects/{ProjectId}")]
    public partial class ProjectView : IDisposable
    {
        [Inject]
        private IAuthService AuthService { get; set; } = null!;

        [Inject]
        private IProjectService ProjectService { get; set; } = null!;

        [Inject]
        private DragDropService DragDrop { get; set; } = null!;

        [Parameter]
        public string? ProjectId { get; set; }

        private bool _isLoading = true;

        private Project? _currentProject;
        private ProjectMember? _currentMember;
        private Sprint? _activeSprint;

        private ProjectViewCategory _activeCategory = ProjectViewCategory.Info;

        protected override async SharpTask OnInitializedAsync()
        {
            _isLoading = true;

            ProjectService.OnProjectStatusHasChanged += ProjectStatusHasChanged;
            ProjectService.OnMemberHasKicked += MemberHaskicked;

            ProjectService.OnSprintHasCreated += SprintHasCreated;
            ProjectService.OnSprintHasFinished += SprintHasFinished;

            DragDrop.OnOverlayNeedsUpdate += () => InvokeAsync(StateHasChanged);
            DragDrop.OnDragStateChanged += () => InvokeAsync(StateHasChanged);
        }

        protected override async SharpTask OnParametersSetAsync()
        {
            _isLoading = true;

            if (!string.IsNullOrWhiteSpace(ProjectId) && Guid.TryParse(ProjectId, out Guid guid))
            {
                _currentProject = await ProjectService.GetProjectByIdAsync(guid);
                if (_currentProject is null) return;

                _currentMember = _currentProject.Members.FirstOrDefault(m => m.UserId == AuthService.CurrentUser?.Id);
                _activeSprint = await ProjectService.GetActiveSprintByProjectIdAsync(_currentProject.Id);
                _activeCategory = ProjectViewCategory.Info;
            }

            _isLoading = false;
        }

        private List<ProjectViewCategory> GetCategoryTabs()
        {
            var tabs = new List<ProjectViewCategory>
            {
                ProjectViewCategory.Info,
                ProjectViewCategory.Backlog,
                ProjectViewCategory.Sprints
            };

            if (_activeSprint is not null)
                tabs.Add(ProjectViewCategory.ActiveSprint);

            return tabs;
        }

        private async SharpTask ChangeCategory(ProjectViewCategory category)
        {
            _activeCategory = category;
        }

        private void ProjectStatusHasChanged(Project updatedProject)
        {
            if (_currentProject?.Id != updatedProject.Id) return;

            _currentProject.Status = updatedProject.Status;
            StateHasChanged();
        }

        private void MemberHaskicked(ProjectMember kickedMember)
        {
            if (_currentProject is not null && _currentProject.Members.Remove(kickedMember))
                StateHasChanged();
        }

        private void SprintHasCreated(Sprint newSprint)
        {
            _activeSprint = newSprint;
            StateHasChanged();
        }

        private void SprintHasFinished()
        {
            _activeCategory = ProjectViewCategory.Sprints;
            _activeSprint = null;
            StateHasChanged();
        }

        public void Dispose()
        {
            ProjectService.OnProjectStatusHasChanged -= ProjectStatusHasChanged;
            ProjectService.OnMemberHasKicked -= MemberHaskicked;

            ProjectService.OnSprintHasCreated -= SprintHasCreated;
            ProjectService.OnSprintHasFinished -= SprintHasFinished;

            DragDrop.OnOverlayNeedsUpdate -= () => InvokeAsync(StateHasChanged);
            DragDrop.OnDragStateChanged -= () => InvokeAsync(StateHasChanged);
        }
    }
}
