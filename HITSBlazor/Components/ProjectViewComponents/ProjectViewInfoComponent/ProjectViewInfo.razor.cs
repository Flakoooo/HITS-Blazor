using HITSBlazor.Components.ActionMenus.BaseActionMenu;
using HITSBlazor.Components.Button;
using HITSBlazor.Components.Modals.CenterModals.FinishProjectModal;
using HITSBlazor.Components.Modals.Components.RightSideModaCollapselInfo;
using HITSBlazor.Components.Tables.TableHeader;
using HITSBlazor.Components.Typography;
using HITSBlazor.Models.Projects.Entities;
using HITSBlazor.Models.Projects.Enums;
using HITSBlazor.Services.Auth;
using HITSBlazor.Services.Modal;
using HITSBlazor.Services.Projects;
using Microsoft.AspNetCore.Components;
using SharpTask = System.Threading.Tasks.Task;

namespace HITSBlazor.Components.ProjectViewComponents.ProjectViewInfoComponent
{
    public partial class ProjectViewInfo
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

        [Parameter]
        public ProjectMember? CurrentProjectMember { get; set; }

        private bool _isLoading = true;

        private string _searchtext = string.Empty;

        private List<ProjectMember> _projectMembers = [];

        private List<CollapseItem> _projectInfoData = [];

        private static readonly List<TableHeaderItem> _membersTableHeader =
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

            _projectInfoData = GetProjectData();

            _isLoading = false;
        }

        protected override void OnParametersSet()
        {
            SeacrhMember(_searchtext);
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
                actions.Add(MenuAction.RemoveTeamMember, member);

            return actions;
        }

        private void SeacrhMember(string value)
        {
            _searchtext = value;
            if (string.IsNullOrWhiteSpace(_searchtext))
                _projectMembers = CurrentProject?.Members.ToList() ?? [];
            else
                _projectMembers = CurrentProject?.Members.Where(m => m.FullName.Contains(_searchtext, StringComparison.CurrentCultureIgnoreCase)).ToList() ?? [];
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
                    () => ProjectService.ActivateProjectAsync(CurrentProject),
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
                    () => ProjectService.DeletedProjectAsync(CurrentProject),
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
            else if (context.Item is ProjectMember member && context.Action is MenuAction.RemoveTeamMember && CurrentProject is not null)
            {
                ModalService.ShowConfirmModal(
                    $"Вы действительно хотите исключить {member.FullName}? Данное действие отменить нельзя.",
                    () => ProjectService.KickMemberFromProjectAsync(CurrentProject.Id, member),
                    questionTextColor: TextColor.Danger,
                    confirmButtonVariant: ButtonVariant.Danger,
                    confirmButtonText: "Исключить пользователя"
                );
            }
        }
    }
}
