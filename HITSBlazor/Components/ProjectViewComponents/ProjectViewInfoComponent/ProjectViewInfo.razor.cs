using HITSBlazor.Components.ActionMenus.BaseActionMenu;
using HITSBlazor.Components.Modals.Components.RightSideModaCollapselInfo;
using HITSBlazor.Components.Tables.TableHeader;
using HITSBlazor.Models.Projects.Entities;
using HITSBlazor.Services.Modal;
using Microsoft.AspNetCore.Components;

using HITSTask = HITSBlazor.Models.Projects.Entities.Task;
using SharpTask = System.Threading.Tasks.Task;

namespace HITSBlazor.Components.ProjectViewComponents.ProjectViewInfoComponent
{
    public partial class ProjectViewInfo
    {
        [Inject]
        private ModalService ModalService { get; set; } = null!;

        [Parameter]
        public bool IsLoading { get; set; } = false;

        [Parameter]
        public Project? CurrentProject { get; set; }

        private string _searchtext = string.Empty;

        private List<CollapseItem> _projectInfoData = [];

        private static List<TableHeaderItem> _membersTableHeader =
        [
            new() { Text = "Почта",                     },
            new() { Text = "Имя",                       },
            new() { Text = "Фамилия",                   },
            new() { Text = "Роль", InCentered = true    }
        ];

        protected override async SharpTask OnInitializedAsync()
        {
            _projectInfoData = GetProjectData();
        }

        private List<CollapseItem> GetProjectData() => [
            new() { Title = "Описание необходимых ресурсов для реализации", Data = CurrentProject?.Description },
        ];

        private async SharpTask SeacrhMember(string value)
        {
            _searchtext = value;
        }

        private void ShowProfileModal(Guid userId) => ModalService.ShowProfileModal(userId);

        private void ShowIdeaModal(Guid? ideaid)
        {
            if (ideaid.HasValue) ModalService.ShowIdeaModal(ideaid.Value);
        }

        private async SharpTask OnMemberAction(TableActionContext context)
        {
            if (context.Action is MenuAction.ViewProfile && context.Item is Guid memberId)
                ShowProfileModal(memberId);
        }
    }
}
