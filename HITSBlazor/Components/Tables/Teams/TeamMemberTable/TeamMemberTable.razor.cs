using HITSBlazor.Components.ActionMenus.BaseActionMenu;
using HITSBlazor.Components.Button;
using HITSBlazor.Components.Tables.TableHeader;
using HITSBlazor.Components.Typography;
using HITSBlazor.Models.Teams.Entities;
using HITSBlazor.Models.Users.Enums;
using HITSBlazor.Services.Auth;
using HITSBlazor.Services.Modal;
using HITSBlazor.Services.Teams;
using Microsoft.AspNetCore.Components;

namespace HITSBlazor.Components.Tables.Teams.TeamMemberTable
{
    public partial class TeamMemberTable
    {
        [Inject]
        private IAuthService AuthService { get; set; } = null!;

        [Inject]
        private ITeamService TeamService { get; set; } = null!;

        [Inject]
        private ModalService ModalService { get; set; } = null!;

        [Parameter]
        public required Team CurrentTeam { get; set; }

        private string _searchText = string.Empty;
        private List<TeamMember> _teamMembers = [];

        private static List<TableHeaderItem> MembersTableHeader { get; } =
        [
            new() { Text = "Почта", ColumnClass = "col-5" },
            new() { Text = "Имя", ColumnClass = "col-3" },
            new() { Text = "Фамилия", ColumnClass = "col-3" }
        ];

        protected override void OnParametersSet()
        {
            SearchData(_searchText);
        }

        private async void SearchData(string value)
        {
            _searchText = value;

            if (string.IsNullOrWhiteSpace(_searchText))
            {
                _teamMembers = CurrentTeam.Members.ToList();
            }
            else
            {
                _teamMembers = CurrentTeam.Members
                    .Where(m => m.FullName.Contains(_searchText, StringComparison.CurrentCultureIgnoreCase))
                    .ToList();
            }
            StateHasChanged();
        }

        private (TextColor? TextColor, string DisplayText) GetRoleDisplayInfo(Guid memberId)
        {
            if (memberId == CurrentTeam.Owner.UserId)
            {
                string displayText = "(Владелец)";
                if (memberId == CurrentTeam.Leader?.UserId)
                    displayText = "(Владелец и Тим-лид)";

                return (TextColor.Warning, displayText);
            }
            else if (memberId == CurrentTeam.Leader?.UserId)
            {
                return (TextColor.Primary, "(Тим-лид)");
            }
            else
            {
                return (null, string.Empty);
            }
        }

        private Dictionary<MenuAction, object> GetActions(TeamMember member)
        {
            var actions = new Dictionary<MenuAction, object>
            {
                [MenuAction.ViewProfile] = member.UserId
            };

            var currentUser = AuthService.CurrentUser;
            if (currentUser is not null)
            {
                if (currentUser.Role is RoleType.Admin || currentUser.Id == CurrentTeam.Owner.UserId || currentUser.Id == CurrentTeam.Leader?.UserId)
                {
                    if (member.UserId == CurrentTeam.Leader?.UserId && member.UserId != CurrentTeam.Owner.UserId)
                        actions.Add(MenuAction.UnsetLeader, member.UserId);
                    else if (member.UserId != CurrentTeam.Leader?.UserId)
                        actions.Add(MenuAction.SetLeader, member);

                    if (member.UserId != CurrentTeam.Owner.UserId)
                        actions.Add(MenuAction.RemoveTeamMember, member);
                }
            }

            return actions;
        }

        private void ShowUserProfile(Guid userId) => ModalService.ShowProfileModal(userId);

        private void HandleTableMenuClick(TableActionContext context)
        {
            if (context.Item is Guid userId)
            {
                if (context.Action is MenuAction.ViewProfile)
                {
                    ShowUserProfile(userId);
                }
            }
            else if (context.Item is TeamMember member)
            {
                if (context.Action is MenuAction.RemoveTeamMember)
                {
                    ModalService.ShowConfirmModal(
                        $"Вы действительно хотите исключить {member.FullName}?",
                        () => TeamService.KickMemberAsync(member),
                        confirmButtonVariant: ButtonVariant.Danger,
                        confirmButtonText: "Удалить"
                    );
                }
                else if (context.Action is MenuAction.SetLeader)
                {
                    //TODOO: добавить что ли склонение какое то имени
                    ModalService.ShowConfirmModal(
                        $"Вы действительно хотите назначить {member.FullName} новым Тим-лидером?",
                        () => TeamService.UpdateTeamLeader(CurrentTeam, member),
                        confirmButtonVariant: ButtonVariant.Warning,
                        confirmButtonText: "Назначить"
                    );
                }
                else if (context.Action is MenuAction.UnsetLeader)
                {
                    //TODOO: добавить что ли склонение какое то имени
                    ModalService.ShowConfirmModal(
                        $"Вы действительно хотите снять {member.FullName} с роли Тим-лидера? Роль Тим-лидера будет назначена владельцу команды",
                        () => TeamService.UpdateTeamLeader(CurrentTeam, null),
                        confirmButtonVariant: ButtonVariant.Warning,
                        confirmButtonText: "Снять"
                    );
                }
            }
        }
    }
}
