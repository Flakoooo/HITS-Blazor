using HITSBlazor.Components.ActionMenus.BaseActionMenu;
using HITSBlazor.Components.Modals.CenterModals.UpdateUserModal;
using HITSBlazor.Components.Modals.CenterModals.UsersGroupModal;
using HITSBlazor.Components.Tables.TableHeader;
using HITSBlazor.Models.Common.Entities;
using HITSBlazor.Models.Users.Entities;
using HITSBlazor.Models.Users.Enums;
using HITSBlazor.Services.UsersGroups;
using HITSBlazor.Services.Modal;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;

namespace HITSBlazor.Pages.Admin.AllUsersGroups
{
    [Authorize]
    [Route("admin/users-groups")]
    public partial class AllUsersGroups : IDisposable
    {
        [Inject]
        private IUsersGroupsService UsersGroupsService { get; set; } = null!;

        [Inject]
        private ModalService ModalService { get; set; } = null!;

        private bool _isLoading = true;

        private string _seacrhText = string.Empty;

        private static List<TableHeaderItem> UsersGroupsTableHeader =>
        [
            new TableHeaderItem { Text = "Название",    ColumnClass = "col-6" },
            new TableHeaderItem { Text = "Роли",        ColumnClass = "col-6" }
        ];

        private List<UsersGroup> _usersGroups = [];
        private HashSet<RoleType> SelectedRoles { get; set; } = [];

        protected override async Task OnInitializedAsync()
        {
            _isLoading = true;

            UsersGroupsService.OnUsersGroupsStateChanged += StateHasChanged;
            await LoadUsersGroupsAsync();

            _isLoading = false;
        }

        private async Task LoadUsersGroupsAsync()
        {
            _usersGroups = await UsersGroupsService.GetUsersGroupsAsync(
                searchText: _seacrhText,
                selectedRoles: SelectedRoles
            );
        }

        private void ShowUsersGroupModal(Guid? usersGroupid = null)
        {
            if (usersGroupid.HasValue)
            {
                ModalService.Show<UsersGroupModal>(
                    ModalType.Center,
                    parameters: new Dictionary<string, object> { [nameof(UsersGroupModal.UsersGroupId)] = usersGroupid }
                );
            }
            else
            {
                ModalService.Show<UsersGroupModal>(ModalType.Center);
            }
        }

        private async Task SearchUsersGroup(string searchText)
        {
            _seacrhText = searchText;
            await LoadUsersGroupsAsync();
        }

        private async Task OnRoleTypeChanged(RoleType role, bool isChecked)
        {
            if (isChecked)
                SelectedRoles.Add(role);
            else
                SelectedRoles.Remove(role);

            await LoadUsersGroupsAsync();
        }

        private async Task ResetFilters()
        {
            SelectedRoles.Clear();
            await LoadUsersGroupsAsync();
        }

        private async Task OnUsersGroupAction(TableActionContext context)
        {
            if (context.Action == MenuAction.Edit)
            {
                if (context.Item is User user)
                    ModalService.Show<UpdateUserModal>(
                        ModalType.Center,
                        parameters: new Dictionary<string, object> { [nameof(UpdateUserModal.UserForUpdate)] = user }
                    );
            }
            else if (context.Action == MenuAction.Delete)
            {
                if (context.Item is not UsersGroup usersGroup || !await UsersGroupsService.DeleteUsersGroupsAsync(usersGroup))
                    return;

                _usersGroups.Remove(usersGroup);
            }
        }

        public void Dispose()
        {
            UsersGroupsService.OnUsersGroupsStateChanged -= StateHasChanged;
        }
    }
}
