using HITSBlazor.Components.ProjectViewComponents.ProjectViewActiveSprintTasks;
using HITSBlazor.Models.Projects.Entities;
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

            DragDrop.OnDragStateChanged += StateHasChanged;

            if (!string.IsNullOrWhiteSpace(ProjectId) && Guid.TryParse(ProjectId, out Guid guid))
            {
                _currentProject = await ProjectService.GetProjectByIdAsync(guid);
                if (_currentProject is null) return;

                _currentMember = await ProjectService.GetCurrentProjectMemberAsync(guid);
                _activeSprint = await ProjectService.GetActiveSprintByProjectIdAsync(_currentProject.Id);
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

        public void Dispose()
        {
            DragDrop.OnDragStateChanged -= StateHasChanged;
        }
    }
}
