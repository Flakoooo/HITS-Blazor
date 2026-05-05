using HITSBlazor.Components.Modals.CenterModals.EndedSprintModal;
using HITSBlazor.Components.Modals.CenterModals.SprintModal;
using HITSBlazor.Components.Tables.TableComponent;
using HITSBlazor.Components.Tables.TableHeader;
using HITSBlazor.Models.Projects.Entities;
using HITSBlazor.Models.Projects.Enums;
using HITSBlazor.Services;
using HITSBlazor.Services.Auth;
using HITSBlazor.Services.Modal;
using HITSBlazor.Services.Projects;
using Microsoft.AspNetCore.Components;

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

        [Inject]
        private GlobalNotificationService NotificationService { get; set; } = null!;

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

            ProjectService.OnSprintHasUpdated += SprintHasUpdated;

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

        protected override async SharpTask OnLoadMoreItemsAsync() => await LoadSprintAsync(append: true);

        private async SharpTask LoadSprintAsync(bool append = false)
        {
            if (CurrentProject is null) return;

            await LoadDataAsync(
                _projectSprints,
                () => ProjectService.GetSprintsByProjectIdAsync(
                    CurrentProject.Id,
                    _currentPage,
                    searchText: _searchText
                ),
                append: append
            );
        }

        private async SharpTask SeacrhSprint(string value)
        {
            _searchText = value;
            await LoadSprintAsync();
        }

        private void SprintHasUpdated(Sprint updatedSprint)
        {
            var sprintForUpdate = _projectSprints.FirstOrDefault(s => s.Id == updatedSprint.Id);
            if (sprintForUpdate is null) return;

            sprintForUpdate.Name = updatedSprint.Name;
            sprintForUpdate.Goal = updatedSprint.Goal;
            sprintForUpdate.StartDate = updatedSprint.StartDate;
            sprintForUpdate.FinishDate = updatedSprint.FinishDate;
            sprintForUpdate.WorkingHours = updatedSprint.WorkingHours;

            StateHasChanged();
        }

        private async SharpTask ShowSprintModal(Sprint? sprint = null)
        {
            if (CurrentProject is null) return;

            if (sprint is not null)
            {
                ModalService.Show(
                    sprint.Status is SprintStatus.Done ? typeof(EndedSprintModal) : typeof(SprintModal), 
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
                if (await ProjectService.GetActiveSprintByProjectIdAsync(CurrentProject.Id) is not null)
                {
                    NotificationService.ShowError("Сперва завершите активный спринт");
                    return;
                }

                ModalService.Show<SprintModal>(
                    ModalType.Center,
                    parameters: new Dictionary<string, object>
                    {
                        [nameof(SprintModal.ProjectId)] = CurrentProject.Id
                    }
                );
            }
        }

        protected override async ValueTask DisposeAsyncCore()
        {
            ProjectService.OnSprintHasUpdated -= SprintHasUpdated;

            await ValueTask.CompletedTask;
        }
    }
}
