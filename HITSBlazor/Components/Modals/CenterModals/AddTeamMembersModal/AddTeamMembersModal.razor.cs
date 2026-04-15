using HITSBlazor.Components.ActionMenus.BaseActionMenu;
using HITSBlazor.Components.Tables.TableHeader;
using HITSBlazor.Models.Teams.Entities;
using HITSBlazor.Models.Users.Entities;
using HITSBlazor.Services;
using HITSBlazor.Services.Modal;
using HITSBlazor.Services.Teams;
using HITSBlazor.Services.Users;
using Microsoft.AspNetCore.Components;

namespace HITSBlazor.Components.Modals.CenterModals.AddTeamMembersModal
{
    public partial class AddTeamMembersModal
    {
        [Inject]
        private IUserService UserService { get; set; } = null!;

        [Inject]
        private ModalService ModalService { get; set; } = null!;

        private string _seacrhText = string.Empty;
        private List<User> _users = [];

        private string SearchSkillText { get; set; } = string.Empty;
        private HashSet<Guid> SelectedSkillIds { get; set; } = [];

        private HashSet<User> _selectedUsers = [];

        private List<TableHeaderItem> _tableHeader = 
        [
            new() { Text = "Почта",     ColumnClass = "col-4" },
            new() { Text = "Имя",       ColumnClass = "col-4" },
            new() { Text = "Фамилия",   ColumnClass = "col-6" }
        ];

        protected override async Task OnInitializedAsync()
        {
            await LoadUsersAsync();
        }

        private async Task LoadUsersAsync()
        {
            _users = await UserService.GetUsersAsync(
                searchText: _seacrhText
            );
        }

        private Dictionary<MenuAction, object> GetActions(User user) => new()
        {
            [MenuAction.ViewProfile] = user.Id,
            [_selectedUsers.Contains(user) 
                ? MenuAction.TeamCreateRemoveMember : MenuAction.TeamCreateSelectMember] = user
        };

        private async Task OnTeamAction(TableActionContext context)
        {
            if (context.Action == MenuAction.ViewProfile && context.Item is Guid userId)
            {
                ModalService.ShowProfileModal(userId);
            }
            else if (context.Item is User user)
            {
                if (context.Action == MenuAction.TeamCreateSelectMember)
                    _selectedUsers.Add(user);
                else if (context.Action == MenuAction.TeamCreateRemoveMember)
                    _selectedUsers.Remove(user);

                StateHasChanged();
            }
        }
    }
}
