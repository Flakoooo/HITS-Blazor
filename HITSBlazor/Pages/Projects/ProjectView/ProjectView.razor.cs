using HITSBlazor.Components.ActionMenus.BaseActionMenu;
using HITSBlazor.Components.Modals.Components.RightSideModaCollapselInfo;
using HITSBlazor.Components.Modals.RightSideModals.TeamModal;
using HITSBlazor.Components.Tables.TableHeader;
using HITSBlazor.Models.Ideas.Entities;
using HITSBlazor.Models.Projects.Entities;
using HITSBlazor.Services;
using HITSBlazor.Services.Modal;
using HITSBlazor.Utils.Mocks.Projects;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;

namespace HITSBlazor.Pages.Projects.ProjectView
{
    [Authorize]
    [Route("projects/{ProjectId}")]
    public partial class ProjectView
    {
        [Inject]
        private ModalService ModalService { get; set; } = null!;

        [Parameter]
        public string? ProjectId { get; set; }

        private bool _isLoading = true;

        private Project? _currentProject;
        private List<Models.Projects.Entities.Task> _projectTasks = [];

        private ProjectViewCategory _activeCategory = ProjectViewCategory.Info;

        private List<CollapseItem> _projectInfoData = [];

        private string _seacrhMemberText = string.Empty;

        private static List<TableHeaderItem> MembersTableHeader { get; } =
        [
            new() { Text = "Почта",                     },
            new() { Text = "Имя",                       },
            new() { Text = "Фамилия",                   },
            new() { Text = "Роль", InCentered = true    }
        ];

        protected override async System.Threading.Tasks.Task OnInitializedAsync()
        {
            _isLoading = true;

            if (!string.IsNullOrWhiteSpace(ProjectId) && Guid.TryParse(ProjectId, out Guid guid))
            {
                _currentProject = MockProjects.GetProjectById(guid);
                if (_currentProject is null) return;

                _projectInfoData = GetProjectData();

                _projectTasks = MockSprints.GetTasksByProjectId(_currentProject.Id);
            }

            _isLoading = false;
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

        private async System.Threading.Tasks.Task OnMemberAction(TableActionContext context)
        {
            if (context.Action == MenuAction.ViewProfile)
            {
                if (context.Item is Guid memberId)
                    ModalService.ShowProfileModal(memberId);
            }
        }
    }
}
