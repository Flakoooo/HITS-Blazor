using HITSBlazor.Components.ActionMenus.BaseActionMenu;
using HITSBlazor.Components.Button;
using HITSBlazor.Components.Modals.CenterModals.UpdateUserModal;
using HITSBlazor.Components.Modals.CenterModals.UsersGroupModal;
using HITSBlazor.Components.Tables.TableComponent;
using HITSBlazor.Components.Tables.TableHeader;
using HITSBlazor.Models.Common.Entities;
using HITSBlazor.Models.Ideas.Entities;
using HITSBlazor.Models.Users.Entities;
using HITSBlazor.Models.Users.Enums;
using HITSBlazor.Services.Auth;
using HITSBlazor.Services.Modal;
using HITSBlazor.Services.UsersGroups;
using HITSBlazor.Utils.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;

namespace HITSBlazor.Pages.Admin.AllUsersGroups
{
    [Authorize]
    [Route("admin/users-groups")]
    public partial class AllUsersGroups
    {
        [Inject]
        private IUsersGroupsService UsersGroupsService { get; set; } = null!;

        [Inject]
        private ModalService ModalService { get; set; } = null!;

        private bool _isLoading = true;

        private TableComponent? _tableComponent;

        private string _seacrhText = string.Empty;

        private static List<TableHeaderItem> UsersGroupsTableHeader =>
        [
            new TableHeaderItem { Text = "Название",    ColumnClass = "col-6" },
            new TableHeaderItem { Text = "Роли",        ColumnClass = "col-6" }
        ];

        private readonly List<UsersGroup> _usersGroups = [];

        private readonly List<EnumViewModel<RoleType>> _filterRoleTypes
            = [.. Enum.GetValues<RoleType>().Select(r => new EnumViewModel<RoleType>(r))];
        private HashSet<EnumViewModel<RoleType>> SelectedRoles { get; set; } = [];

        protected override async Task OnInitializedAsync()
        {
            _isLoading = true;

            UsersGroupsService.OnUsersGroupHasCreated += UsersGroupHasCreated;
            UsersGroupsService.OnUsersGroupHasUpdated += UsersGroupHasUpdated;
            UsersGroupsService.OnUsersGroupHasDeleted += UsersGroupHasDeleted;

            await LoadUsersGroupsAsync();

            _isLoading = false;
            MarkAsInitialized();
        }

        protected override async Task AdditionalAfterRenderMethod()
        {
            if (_tableComponent != null)
                _tableContainer = _tableComponent.ScrollContainer;
        }

        protected override int GetCurrentItemsCount() => _usersGroups.Count;

        protected override async Task OnLoadMoreItemsAsync()
        {
            await LoadUsersGroupsAsync(append: true);
        }

        private async Task LoadUsersGroupsAsync(bool append = false) => await LoadDataAsync(
            _usersGroups,
            () => UsersGroupsService.GetUsersGroupsAsync(
                _currentPage,
                searchText: _seacrhText,
                selectedRoles: SelectedRoles.Select(r => r.Value)
            ),
            append: append
        );

        private void ShowUsersGroupModal(Guid? usersGroupid = null) => ModalService.Show<UsersGroupModal>(
            ModalType.Center,
            parameters: usersGroupid.HasValue
                ? new Dictionary<string, object> { [nameof(UsersGroupModal.UsersGroupId)] = usersGroupid.Value }
                : null
        );

        private async Task SearchUsersGroup(string searchText)
        {
            _seacrhText = searchText;
            ResetPagination();
            await LoadUsersGroupsAsync();
        }

        private async Task FiltersHasChanged()
        {
            ResetPagination();
            await LoadUsersGroupsAsync();
        }

        private async Task ResetFilters()
        {
            SelectedRoles.Clear();
            ResetPagination();
            await LoadUsersGroupsAsync();
        }

        private async Task OnUsersGroupAction(TableActionContext context)
        {
            if (context.Action == MenuAction.Edit && context.Item is User user)
            {
                ModalService.Show<UpdateUserModal>(
                    ModalType.Center,
                    parameters: new Dictionary<string, object> { [nameof(UpdateUserModal.UserForUpdate)] = user }
                );
            }
            else if (context.Action == MenuAction.Delete)
            {
                if (context.Item is not UsersGroup usersGroup) return;

                ModalService.ShowConfirmModal(
                    $"Вы действительно хотите удалить {usersGroup.Name}?",
                    () => UsersGroupsService.DeleteUsersGroupsAsync(usersGroup),
                    confirmButtonVariant: ButtonVariant.Danger,
                    confirmButtonText: "Удалить"
                );
            }
        }

        private void UsersGroupHasCreated(UsersGroup newUsersGroup)
        {
            _usersGroups.Add(newUsersGroup);
            ++_totalCount;
            StateHasChanged();
        }

        private void UsersGroupHasUpdated(UsersGroup updatedUsersGroup)
        {
            var usersGroup = _usersGroups.FirstOrDefault(ug => ug.Id == updatedUsersGroup.Id);
            if (usersGroup is null) return;

            usersGroup.Name = updatedUsersGroup.Name;
            usersGroup.Members = updatedUsersGroup.Members;
            usersGroup.Roles = updatedUsersGroup.Roles;

            StateHasChanged();
        }

        private void UsersGroupHasDeleted(UsersGroup usersGroup)
        {
            if (_usersGroups.Remove(usersGroup))
            {
                --_totalCount;
                StateHasChanged();
            }
        }

        protected override async ValueTask DisposeAsyncCore()
        {
            UsersGroupsService.OnUsersGroupHasCreated -= UsersGroupHasCreated;
            UsersGroupsService.OnUsersGroupHasUpdated -= UsersGroupHasUpdated;
            UsersGroupsService.OnUsersGroupHasDeleted -= UsersGroupHasDeleted;

            await ValueTask.CompletedTask;
        }
    }
}
