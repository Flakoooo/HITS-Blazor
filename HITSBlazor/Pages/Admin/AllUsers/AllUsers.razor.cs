using HITSBlazor.Components.ActionMenus.BaseActionMenu;
using HITSBlazor.Components.Button;
using HITSBlazor.Components.Modals.CenterModals.UpdateUserModal;
using HITSBlazor.Components.Tables.TableComponent;
using HITSBlazor.Components.Tables.TableHeader;
using HITSBlazor.Models.Ideas.Entities;
using HITSBlazor.Models.Users.Entities;
using HITSBlazor.Models.Users.Enums;
using HITSBlazor.Services.Auth;
using HITSBlazor.Services.Modal;
using HITSBlazor.Services.Users;
using HITSBlazor.Utils.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;

namespace HITSBlazor.Pages.Admin.AllUsers
{
    [Authorize]
    [Route("/admin/users")]
    public partial class AllUsers
    {
        [Inject]
        private IAuthService AuthService { get; set; } = null!;

        [Inject]
        private IUserService UserService { get; set; } = null!;

        [Inject]
        private ModalService ModalService { get; set; } = null!;

        private bool _isLoading = true;

        private TableComponent? _tableComponent;

        private string SeacrhText { get; set; } = string.Empty;
        private string? _orderBy = null;
        private bool? _byDescending = null;
        private HashSet<EnumViewModel<RoleType>> SelectedUserRoles { get; set; } = [];

        private readonly List<ValueViewModel<bool?>> _inTeamFilterValues =
        [
            new(true, "В команде"),
            new(false, "Не в команде")
        ];

        private ValueViewModel<bool?>? InTeam { get; set; }

        private static readonly List<TableHeaderItem> _userTableHeader =
        [
            new() { Text = "Почта"                                                                  },
            new() { Text = "Имя"                                                                    },
            new() { Text = "Фамилия"                                                                },
            new() { Text = "Номер телефона"                                                         },
            new() { Text = "Учебная группа"                                                         },
            new() { Text = "Дата регистрации", InCentered = true, OrderBy = nameof(User.CreatedAt)  },
            new() { Text = "Роли",             ColumnClass="col-5"                                  }
        ];

        private readonly List<EnumViewModel<RoleType>> _filterRoleTypes
            = [.. Enum.GetValues<RoleType>().Select(s => new EnumViewModel<RoleType>(s))];

        private readonly List<User> _users = [];

        protected override async Task OnInitializedAsync()
        {
            _isLoading = true;

            UserService.OnUserHasUpdated += UserHasUpdated;
            UserService.OnUserHasDeleted += UserHasDeleted;

            await LoadUsersAsync();

            _isLoading = false;
            MarkAsInitialized();
        }

        protected override async Task AdditionalAfterRenderMethod()
        {
            if (_tableComponent != null)
                _tableContainer = _tableComponent.ScrollContainer;
        }

        protected override int GetCurrentItemsCount() => _users.Count;

        protected override async Task OnLoadMoreItemsAsync()
        {
            await LoadUsersAsync(append: true);
        }

        private async Task LoadUsersAsync(bool append = false) => await LoadDataAsync(
            _users,
            () => UserService.GetUsersAsync(
                _currentPage,
                searchText: SeacrhText,
                orderBy: _orderBy,
                byDescending: _byDescending,
                selectedRoles: [.. SelectedUserRoles.Select(s => s.Value)]
            ),
            append: append
        );

        private async Task SearchUser(string searchText)
        {
            SeacrhText = searchText;
            ResetPagination();
            await LoadUsersAsync();
        }

        private async Task SortUser(string? value, bool? state)
        {
            _orderBy = value;
            _byDescending = state;
            ResetPagination();
            await LoadUsersAsync();
        }

        private async Task FiltersHasChanged()
        {
            ResetPagination();
            await LoadUsersAsync();
        }

        private async Task ResetFilters()
        {
            SelectedUserRoles.Clear();
            InTeam = null;

            foreach (var header in _userTableHeader) 
                header.IsOrdered = null;

            ResetPagination();
            await LoadUsersAsync();
        }

        private void ShowUserProfile(Guid userId) => ModalService.ShowProfileModal(userId);

        private Dictionary<MenuAction, object> GetActions(User user)
        {
            if (AuthService.CurrentUser?.Role is RoleType.Admin)
            {
                return new Dictionary<MenuAction, object>
                {
                    [MenuAction.ViewProfile] = user.Id,
                    [MenuAction.Edit] = user,
                    [MenuAction.Delete] = user
                };
            }
            else if (AuthService.CurrentUser?.Role is RoleType.Teacher)
            {
                return new Dictionary<MenuAction, object>
                {
                    [MenuAction.ViewProfile] = user.Id
                };
            }

            return [];
        }

        private async Task OnUserAction(TableActionContext context)
        {
            if (context.Action == MenuAction.ViewProfile)
            {
                if (context.Item is Guid guid)
                    ShowUserProfile(guid);
            }
            else if (context.Item is User user)
            {
                if (context.Action == MenuAction.Edit)
                {
                    ModalService.Show<UpdateUserModal>(
                        ModalType.Center,
                        parameters: new Dictionary<string, object> { [nameof(UpdateUserModal.UserForUpdate)] = user }
                    );
                }
                else if (context.Action == MenuAction.Delete)
                {
                    ModalService.ShowConfirmModal(
                        $"Вы действительно хотите удалить {user.FullName}?",
                        () => UserService.DeleteUserAsync(user),
                        confirmButtonVariant: ButtonVariant.Danger,
                        confirmButtonText: "Удалить"
                    );
                }
            }
        }

        private void UserHasUpdated(User updatedUser)
        {
            var user = _users.FirstOrDefault(c => c.Id == updatedUser.Id);
            if (user is null) return;

            user.Email = updatedUser.Email;
            user.StudyGroup = updatedUser.StudyGroup;
            user.FirstName = updatedUser.FirstName;
            user.LastName = updatedUser.LastName;
            user.Telephone = updatedUser.Telephone;
            user.Roles = updatedUser.Roles;

            StateHasChanged();
        }

        private void UserHasDeleted(User user)
        {
            if (_users.Remove(user))
            {
                --_totalCount;
                StateHasChanged();
            }
        }

        protected override async ValueTask DisposeAsyncCore()
        {
            UserService.OnUserHasUpdated -= UserHasUpdated;
            UserService.OnUserHasDeleted -= UserHasDeleted;

            await ValueTask.CompletedTask;
        }
    }
}
