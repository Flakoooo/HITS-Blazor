using HITSBlazor.Components.ActionMenus.BaseActionMenu;
using HITSBlazor.Components.Button;
using HITSBlazor.Components.Tables.TableHeader;
using HITSBlazor.Models.Projects.Entities;
using HITSBlazor.Models.Projects.Enums;
using HITSBlazor.Services;
using HITSBlazor.Utils.Mocks.Projects;
using HITSBlazor.Utils.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;

namespace HITSBlazor.Pages.Projects.ProjectsList
{
    [Authorize]
    [Route("projects/list")]
    public partial class ProjectsList
    {
        [Inject]
        private NavigationService NavigationService { get; set; } = null!;

        private bool _isLoading = true;

        private string _searchText = string.Empty;

        private List<Project> _projects = [];

        private EnumViewModel<ProjectStatus>? SelectedProjectStatus { get; set; }

        private readonly List<TableHeaderItem> _projectsTableHeader =
        [
            new TableHeaderItem { Text = "Название",        ColumnClass = "col-5"   },
            new TableHeaderItem { Text = "Статус",          InCentered = true       },
            new TableHeaderItem { Text = "Дата старта",     InCentered = true       },
            new TableHeaderItem { Text = "Дата окончания",  InCentered = true       }
        ];

        private readonly List<EnumViewModel<ProjectStatus>> _filterProjectStatus
            = [.. Enum.GetValues<ProjectStatus>().Select(s => new EnumViewModel<ProjectStatus>(s))];

        protected override async System.Threading.Tasks.Task OnInitializedAsync()
        {
            _isLoading = true;

            await LoadProjectsAsync();

            _isLoading = false;
        }

        private async System.Threading.Tasks.Task LoadProjectsAsync()
        {
            _projects = MockProjects.GetAllProjects();
        }

        private async System.Threading.Tasks.Task SearchProject(string value)
        {
            _searchText = value;
            await LoadProjectsAsync();
        }

        private async System.Threading.Tasks.Task NavigateToProject(Guid projectId)
            => await NavigationService.NavigateToAsync($"projects/{projectId}");

        private async System.Threading.Tasks.Task OnProjectAction(TableActionContext context)
        {
            if (context.Action == MenuAction.ViewProject)
            {
                if (context.Item is Guid projectId)
                    await NavigateToProject(projectId);
            }
        }

        private async System.Threading.Tasks.Task ResetFilters()
        {
            SelectedProjectStatus = null;

            await LoadProjectsAsync();
        }
    }
}
