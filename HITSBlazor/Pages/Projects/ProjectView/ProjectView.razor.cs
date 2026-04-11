using HITSBlazor.Components.ActionMenus.BaseActionMenu;
using HITSBlazor.Components.Modals.CenterModals.FinishSprintModal;
using HITSBlazor.Components.Modals.CenterModals.SprintModal;
using HITSBlazor.Components.Modals.CenterModals.TaskModal;
using HITSBlazor.Components.Modals.Components.RightSideModaCollapselInfo;
using HITSBlazor.Components.Modals.RightSideModals.TeamModal;
using HITSBlazor.Components.Tables.TableHeader;
using HITSBlazor.Models.Common.Entities;
using HITSBlazor.Models.Ideas.Entities;
using HITSBlazor.Models.Projects.Entities;
using HITSBlazor.Models.Projects.Enums;
using HITSBlazor.Models.Users.Entities;
using HITSBlazor.Services;
using HITSBlazor.Services.Auth;
using HITSBlazor.Services.Modal;
using HITSBlazor.Utils.Mocks.Common;
using HITSBlazor.Utils.Mocks.Projects;
using HITSBlazor.Utils.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;

namespace HITSBlazor.Pages.Projects.ProjectView
{
    [Authorize]
    [Route("projects/{ProjectId}")]
    public partial class ProjectView
    {
        [Inject]
        private IAuthService AuthService { get; set; } = null!;

        [Inject]
        private ModalService ModalService { get; set; } = null!;

        [Parameter]
        public string? ProjectId { get; set; }

        private bool _isLoading = true;

        private Project? _currentProject;
        private List<Models.Projects.Entities.Task> _projectTasks = [];
        private List<Sprint> _projectSprints = [];
        private Sprint? _activeSprint;

        private ProjectViewCategory _activeCategory = ProjectViewCategory.Info;

        private List<CollapseItem> _projectInfoData = [];

        private List<Tag> _filterTags = [];
        private HashSet<Tag> SelectedTagNames { get; set; } = [];

        private string _seacrhMemberText = string.Empty;
        private string SearchTagFilterText { get; set; } = string.Empty;
        private string _searchSprintText = string.Empty;

        private static List<TableHeaderItem> _membersTableHeader =
        [
            new() { Text = "Почта",                     },
            new() { Text = "Имя",                       },
            new() { Text = "Фамилия",                   },
            new() { Text = "Роль", InCentered = true    }
        ];

        private static List<TableHeaderItem> _sprintsTableHeader =
        [
            new() { Text = "Название",          ColumnClass = "col-3"   },
            new() { Text = "Статус",            InCentered = true       },
            new() { Text = "Дата старта",       InCentered = true       },
            new() { Text = "Дата окончания",    InCentered = true       }
        ];

        private static string GetHintTaskStatusText(Models.Projects.Enums.TaskStatus status) => status switch
        {
            Models.Projects.Enums.TaskStatus.OnModification => "Здесь находятся задачи, которые были отправлены на доработку для исправления ошибок или улучшения качества."
                + " Эти задачи нужно выполнить в первую очередь, чтобы не затягивать сроки проекта.",
            Models.Projects.Enums.TaskStatus.NewTask => "Здесь находятся задачи, которые еще не были назначены разработчику."
                + " Эти задачи можно выбирать по своему усмотрению, учитывая приоритеты и сложность.",
            Models.Projects.Enums.TaskStatus.InProgress => "Здесь находятся задачи, которые в данный момент выполняются командой или отдельным разработчиком."
                + " Данные задачи нужно довести до конца и не переключаться на другие.",
            Models.Projects.Enums.TaskStatus.OnVerification => "Здесь находятся задачи, которые были выполнены и отправлены тимлиду на проверку качества, функциональности и требованиям.",
            Models.Projects.Enums.TaskStatus.Done => "Здесь находятся задачи, которые были успешно проверены и одобрены."
                + " Этизадачи можно считать завершенными и не требующими дальнейшего внимания.",
            _ => $"{nameof(status)} hint text"
        };

        protected override async System.Threading.Tasks.Task OnInitializedAsync()
        {
            _isLoading = true;

            if (!string.IsNullOrWhiteSpace(ProjectId) && Guid.TryParse(ProjectId, out Guid guid))
            {
                _currentProject = MockProjects.GetProjectById(guid);
                if (_currentProject is null) return;

                _projectInfoData = GetProjectData();

                await LoadTasksAsync();
                //лучше наверно заменить на один запрос, который получает все теги проекта
                _filterTags = [.. MockTags.GetTags()];

                _projectSprints = MockSprints.GetSprintsByProjectId(guid);

                _activeSprint = _projectSprints.FirstOrDefault(s => s.Status is SprintStatus.Active);
            }

            _isLoading = false;
        }

        private async System.Threading.Tasks.Task LoadTasksAsync()
        {
            if (_currentProject is null) return;

            if (SelectedTagNames.Count == 0) _projectTasks = MockSprints.GetTasksByProjectId(_currentProject.Id);
            else _projectTasks = [.. MockSprints.GetTasksByProjectId(_currentProject.Id)
                .Where(task => task.Tags.Any(t => SelectedTagNames.Contains(t))
            )];
        }

        private string GetTableCategoryClass(ProjectViewCategory category)
            => _activeCategory == category ? "active text-primary" : "text-secondary";

        private List<CollapseItem> GetProjectData() => [
            new() { Title = "Описание необходимых ресурсов для реализации", Data = _currentProject?.Description },
        ];

        private async System.Threading.Tasks.Task ChangeCategory(ProjectViewCategory category)
        {
            _activeCategory = category;
        }

        private async System.Threading.Tasks.Task SeacrhMember(string value)
        {
            _seacrhMemberText = value;
        }

        private async System.Threading.Tasks.Task SeacrhSprint(string value)
        {
            _searchSprintText = value;
        }

        private string GetTaskExecuteColor(User? executor)
        {
            if (AuthService.CurrentUser is not null 
                && executor is not null 
                && executor.Id == AuthService.CurrentUser.Id
            ) return "13, 110, 253";

            return "158, 158, 158";
        }

        private void ShowProfileModal(Guid userId) => ModalService.ShowProfileModal(userId);

        private void ShowTaskModal(Models.Projects.Entities.Task? task = null) => ModalService.ShowTaskModal(task);

        private void ShowSprintModal(Sprint? sprint = null)
        {
            if (_currentProject is null) return;

            var parameters = new Dictionary<string, object>
            {
                [nameof(SprintModal.ProjectId)] = _currentProject.Id
            };

            if (sprint is not null)
                parameters.Add(nameof(SprintModal.CurrentSprint), sprint);

            ModalService.Show<SprintModal>(
                ModalType.Center,
                parameters: parameters
            );
        }

        private void ShowFinishSprintModal() => ModalService.Show<FinishSprintModal>(
            ModalType.Center,
            parameters: new Dictionary<string, object> { 
                [nameof(FinishSprintModal.ProjectMembers)] = _currentProject?.Members ?? []
            }
        );


        private async System.Threading.Tasks.Task OnMemberAction(TableActionContext context)
        {
            if (context.Action == MenuAction.ViewProfile)
            {
                if (context.Item is Guid memberId)
                    ShowProfileModal(memberId);
            }
        }
    }
}
