using HITSBlazor.Models.Projects.Entities;
using HITSBlazor.Services.Projects;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;

using HITSTask = HITSBlazor.Models.Projects.Entities.Task;
using SharpTask = System.Threading.Tasks.Task;

namespace HITSBlazor.Pages.Projects.ProjectView
{
    [Authorize]
    [Route("projects/{ProjectId}")]
    public partial class ProjectView
    {
        [Inject]
        private IProjectService ProjectService { get; set; } = null!;

        [Parameter]
        public string? ProjectId { get; set; }

        private bool _isLoading = true;

        private Project? _currentProject;
        private Sprint? _activeSprint;

        private ProjectViewCategory _activeCategory = ProjectViewCategory.Info;

        protected override async SharpTask OnInitializedAsync()
        {
            _isLoading = true;

            if (!string.IsNullOrWhiteSpace(ProjectId) && Guid.TryParse(ProjectId, out Guid guid))
            {
                _currentProject = await ProjectService.GetProjectByIdAsync(guid);
                if (_currentProject is null) return;

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
    }
}
