using HITSBlazor.Components.ActionMenus.BaseActionMenu;
using HITSBlazor.Components.Tables.TableComponent;
using HITSBlazor.Components.Tables.TableHeader;
using HITSBlazor.Models.Projects.Entities;
using HITSBlazor.Models.Projects.Enums;
using HITSBlazor.Services;
using HITSBlazor.Services.Projects;
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
        private IProjectService ProjectService { get; set; } = null!;

        [Inject]
        private NavigationService NavigationService { get; set; } = null!;

        private bool _isLoading = true;

        private TableComponent? _tableComponent;

        private string _searchText = string.Empty;

        private readonly List<Project> _projects = [];

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
            MarkAsInitialized();
        }

        protected override async System.Threading.Tasks.Task AdditionalAfterRenderMethod()
        {
            if (_tableComponent != null)
                _tableContainer = _tableComponent.ScrollContainer;
        }

        protected override int GetCurrentItemsCount() => _projects.Count;

        protected override async System.Threading.Tasks.Task OnLoadMoreItemsAsync()
        {
            await LoadProjectsAsync(append: true);
        }

        private async System.Threading.Tasks.Task LoadProjectsAsync(bool append = false) => await LoadDataAsync(
            _projects,
            () => ProjectService.GetProjectsByQueryAsync(
                _currentPage, searchText: _searchText, selectedStatus: SelectedProjectStatus?.Value
            ),
            append: append
        );

        private async System.Threading.Tasks.Task FiltersHasChanged()
        {
            ResetPagination();
            await LoadProjectsAsync();
        }

        private async System.Threading.Tasks.Task SearchProject(string value)
        {
            _searchText = value;
            await FiltersHasChanged();
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

            await FiltersHasChanged();
        }
    }
}
