using HITSBlazor.Components.ActionMenus.BaseActionMenu;
using HITSBlazor.Components.Button;
using HITSBlazor.Components.Modals.CenterModals.FinishProjectModal;
using HITSBlazor.Components.Modals.Components.RightSideModaCollapselInfo;
using HITSBlazor.Components.Tables.TableComponent;
using HITSBlazor.Components.Tables.TableHeader;
using HITSBlazor.Components.Typography;
using HITSBlazor.Models.Ideas.Entities;
using HITSBlazor.Models.Markets.Entities;
using HITSBlazor.Models.Markets.Enums;
using HITSBlazor.Models.Projects.Entities;
using HITSBlazor.Models.Projects.Enums;
using HITSBlazor.Services.Markets;
using HITSBlazor.Services.Modal;
using HITSBlazor.Services.Projects;
using Microsoft.AspNetCore.Components;
using HITSTask = HITSBlazor.Models.Projects.Entities.Task;
using SharpTask = System.Threading.Tasks.Task;

namespace HITSBlazor.Components.ProjectViewComponents.ProjectViewInfoComponent
{
    public partial class ProjectViewInfo
    {
        [Inject]
        private IProjectService ProjectService { get; set; } = null!;

        [Inject]
        private ModalService ModalService { get; set; } = null!;

        [Parameter]
        public bool IsLoading { get; set; } = false;

        [Parameter]
        public Project? CurrentProject { get; set; }

        [Parameter]
        public ProjectMember? CurrentProjectMember { get; set; }

        private bool _isLoading = true;

        private TableComponent? _tableComponent;

        private string _searchtext = string.Empty;

        private readonly List<ProjectMember> _projectMembers = [];

        private List<CollapseItem> _projectInfoData = [];

        private static List<TableHeaderItem> _membersTableHeader =
        [
            new() { Text = "Почта",                     },
            new() { Text = "Имя",                       },
            new() { Text = "Фамилия",                   },
            new() { Text = "Роль", InCentered = true    }
        ];

        private bool IsOnLoading => _isLoading || IsLoading;

        protected override async SharpTask OnInitializedAsync()
        {
            _isLoading = true;

            await LoadProjectMembersAsync();
            _projectInfoData = GetProjectData();

            _isLoading = false;
            MarkAsInitialized();
        }

        protected override async SharpTask AdditionalAfterRenderMethod()
        {
            if (_tableComponent is not null)
                _tableContainer = _tableComponent.ScrollContainer;
        }

        protected override int GetCurrentItemsCount() => _projectMembers.Count;

        protected override async SharpTask OnLoadMoreItemsAsync()
            => await LoadProjectMembersAsync(true);

        private async SharpTask LoadProjectMembersAsync(bool append = false)
        {
            if (CurrentProject is null) return;

            await LoadDataAsync(
                _projectMembers,
                () => ProjectService.GetProjectMembersAsync(
                    CurrentProject.Id,
                    _currentPage,
                    searchText: _searchtext
                ),
                append
            );
        }

        private List<CollapseItem> GetProjectData() => [
            new() { Title = "Описание необходимых ресурсов для реализации", Data = CurrentProject?.Description },
        ];

        private Dictionary<MenuAction, object> GetActions(ProjectMember member)
        {
            var actions = new Dictionary<MenuAction, object>
            {
                [MenuAction.ViewProfile] = member.UserId
            };

            if (CurrentProjectMember?.ProjectRole is ProjectMemberRole.Initiator or ProjectMemberRole.TeamLeader)
            {
                actions.Add(MenuAction.SetLeader, member);
                actions.Add(MenuAction.RemoveTeamMember, member);
            }

            return actions;
        }

        private async SharpTask SeacrhMember(string value)
        {
            _searchtext = value;
            ResetPagination();
            await LoadProjectMembersAsync();
        }

        private void ShowProfileModal(Guid userId) => ModalService.ShowProfileModal(userId);

        private void ShowIdeaModal(Guid? ideaid)
        {
            if (ideaid.HasValue) ModalService.ShowIdeaModal(ideaid.Value);
        }

        private async SharpTask UpdateProjectStatus(ProjectStatus newStatus)
        {
            if (CurrentProject is null) return;

            if (newStatus is ProjectStatus.Paused)
            {
                ModalService.ShowConfirmModal(
                    "Вы действительно хотите остановить проект? Вы сможете возобновить его работу позже.",
                    () => ProjectService.PauseProjectAsync(CurrentProject),
                    questionTextColor: TextColor.Danger,
                    confirmButtonVariant: ButtonVariant.Warning,
                    confirmButtonText: "Остановить проект"
                );
            }
            else if (newStatus is ProjectStatus.Active)
            {
                ModalService.ShowConfirmModal(
                    "Вы действительно хотите запустить проект?",
                    () => ProjectService.PauseProjectAsync(CurrentProject),
                    questionTextColor: TextColor.Danger,
                    confirmButtonVariant: ButtonVariant.Success,
                    confirmButtonText: "Запустить проект"
                );
            }
            else if (newStatus is ProjectStatus.Done)
            {
                var parameters = new Dictionary<string, object>
                {
                    [nameof(FinishProjectModal.CurrentProject)] = CurrentProject
                };

                if (CurrentProjectMember is not null)
                    parameters.Add(nameof(FinishProjectModal.CurrentProjectMember), CurrentProjectMember);

                ModalService.Show<FinishProjectModal>(ModalType.Center, parameters: parameters);
            }
            else if (newStatus is ProjectStatus.Deleted)
            {
                ModalService.ShowConfirmModal(
                    "Вы действительно хотите удалить проект? Данное действие отменить нельзя.",
                    () => ProjectService.PauseProjectAsync(CurrentProject),
                    questionTextColor: TextColor.Danger,
                    confirmButtonVariant: ButtonVariant.Danger,
                    confirmButtonText: "Удалить проект"
                );
            }
        }

        private async SharpTask OnMemberAction(TableActionContext context)
        {
            if (context.Action is MenuAction.ViewProfile && context.Item is Guid memberId)
                ShowProfileModal(memberId);
        }
    }
}
