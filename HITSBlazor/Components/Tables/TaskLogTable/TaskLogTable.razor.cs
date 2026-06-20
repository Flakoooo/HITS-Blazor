using HITSBlazor.Components.Tables.TableHeader;
using HITSBlazor.Models.Projects.Entities;
using HITSBlazor.Services.Projects;
using Microsoft.AspNetCore.Components;

using ShrapTask = System.Threading.Tasks.Task;

namespace HITSBlazor.Components.Tables.TaskLogTable
{
    public partial class TaskLogTable
    {
        [Inject]
        private IProjectService ProjectService { get; set; } = null!;

        [Parameter]
        public bool IsLoading { get; set; } = false;

        [Parameter]
        public Guid TaskId { get; set; }

        private bool IsOnLoading => _isLoading || IsLoading;

        private bool _isLoading = true;

        private TableComponent.TableComponent? _tableComponent;

        private readonly List<TaskMovementLog> _taskLogs = [];

        private readonly List<TableHeaderItem> _tableTaskLogHeader =
        [
            new() { Text = "Статус",            InCentered = true,  ColumnClass = "col-3"   },
            new() { Text = "Дата вступления",   InCentered = true                           },
            new() { Text = "Дата окончания",    InCentered = true                           },
            new() { Text = "Продолжительность", InCentered = true                           }
        ];

        protected override async ShrapTask OnInitializedAsync()
        {
            _isLoading = true;

            _isLoading = false;

            MarkAsInitialized();
        }

        protected override async ShrapTask OnParametersSetAsync()
        {
            await LoadTasksLogsAsync();
        }

        protected override async ShrapTask AdditionalAfterRenderMethod()
        {
            if (_tableComponent != null)
                _tableContainer = _tableComponent.ScrollContainer;
        }

        protected override int GetCurrentItemsCount() => _taskLogs.Count;

        protected override async ShrapTask OnLoadMoreItemsAsync()
        {
            await LoadTasksLogsAsync(true);
        }

        private async ShrapTask LoadTasksLogsAsync(bool append = false) => await LoadDataAsync(
            _taskLogs,
            () => ProjectService.GetTasksLogsAsync(TaskId, _currentPage),
            append: append
        );
    }
}
