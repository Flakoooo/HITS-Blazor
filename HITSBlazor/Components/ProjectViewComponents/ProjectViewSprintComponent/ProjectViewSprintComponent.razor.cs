using HITSBlazor.Components.Modals.CenterModals.EndedSprintModal;
using HITSBlazor.Components.Modals.CenterModals.SprintModal;
using HITSBlazor.Components.Tables.TableComponent;
using HITSBlazor.Components.Tables.TableHeader;
using HITSBlazor.Models.Projects.Entities;
using HITSBlazor.Models.Projects.Enums;
using HITSBlazor.Services.Auth;
using HITSBlazor.Services.Modal;
using HITSBlazor.Services.Projects;
using Microsoft.AspNetCore.Components;

using HITSTask = HITSBlazor.Models.Projects.Entities.Task;
using SharpTask = System.Threading.Tasks.Task;

namespace HITSBlazor.Components.ProjectViewComponents.ProjectViewSprintComponent
{
    public partial class ProjectViewSprintComponent
    {
        [Inject]
        private IAuthService AuthService { get; set; } = null!;

        [Inject]
        private IProjectService ProjectService { get; set; } = null!;

        [Inject]
        private ModalService ModalService { get; set; } = null!;

        [Parameter]
        public bool IsLoading { get; set; } = false;

        [Parameter]
        public Project? CurrentProject { get; set; }

        private bool IsOnLoading => _isLoading || IsLoading;

        private bool _isLoading = true;

        private TableComponent? _tableComponent;

        private string _searchText = string.Empty;

        private readonly List<Sprint> _projectSprints = [];

        private static readonly List<TableHeaderItem> _sprintsTableHeader =
        [
            new() { Text = "Название",          ColumnClass = "col-3"   },
            new() { Text = "Статус",            InCentered = true       },
            new() { Text = "Дата старта",       InCentered = true       },
            new() { Text = "Дата окончания",    InCentered = true       }
        ];

        protected override async SharpTask OnInitializedAsync()
        {
            _isLoading = true;

            await LoadSprintAsync();

            _isLoading = false;
            MarkAsInitialized();
        }

        protected override async SharpTask AdditionalAfterRenderMethod()
        {
            if (_tableComponent != null)
                _tableContainer = _tableComponent.ScrollContainer;
        }

        protected override int GetCurrentItemsCount() => _projectSprints.Count;

        protected override async SharpTask OnLoadMoreItemsAsync()
        {
            await LoadSprintAsync(append: true);
        }

        private async SharpTask LoadSprintAsync(bool append = false)
        {
            if (CurrentProject is not null)
            {
                await LoadDataAsync(
                    _projectSprints,
                    () => ProjectService.GetSprintsByProjectIdAsync(
                        CurrentProject.Id, _currentPage
                    ),
                    append: append
                );
            }
        }

        private async SharpTask SeacrhSprint(string value)
        {
            _searchText = value;
        }

        private void ShowSprintModal(Sprint? sprint = null)
        {
            if (CurrentProject is null) return;

            if (sprint is not null)
            {
                if (sprint.Status is SprintStatus.Done)
                {
                    ModalService.Show<EndedSprintModal>(
                        ModalType.Center,
                        parameters: new Dictionary<string, object>
                        {
                            [nameof(EndedSprintModal.ProjectId)] = CurrentProject.Id,
                            [nameof(EndedSprintModal.CurrentSprint)] = sprint
                        }
                    );
                }
                else
                {
                    ModalService.Show<SprintModal>(
                        ModalType.Center,
                        parameters: new Dictionary<string, object>
                        {
                            [nameof(SprintModal.ProjectId)] = CurrentProject.Id,
                            [nameof(SprintModal.CurrentSprint)] = sprint
                        }
                    );
                }
            }
            else
            {
                ModalService.Show<SprintModal>(
                    ModalType.Center,
                    parameters: new Dictionary<string, object>
                    {
                        [nameof(SprintModal.ProjectId)] = CurrentProject.Id
                    }
                );
            }
        }
    }
}
