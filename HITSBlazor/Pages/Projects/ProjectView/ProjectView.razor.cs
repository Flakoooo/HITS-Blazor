using HITSBlazor.Components.Modals.Components.RightSideModaCollapselInfo;
using HITSBlazor.Components.Modals.RightSideModals.TeamModal;
using HITSBlazor.Components.Tables.TableHeader;
using HITSBlazor.Models.Projects.Entities;
using Microsoft.AspNetCore.Components;

namespace HITSBlazor.Pages.Projects.ProjectView
{
    public partial class ProjectView
    {
        [Parameter]
        public string? ProjectId { get; set; }

        private bool _isLoading = true;

        private Project? _currentProject;

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

            _projectInfoData = GetProjectData();

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
    }
}
