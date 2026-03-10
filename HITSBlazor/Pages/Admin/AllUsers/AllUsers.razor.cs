using HITSBlazor.Components.ActionMenus.BaseActionMenu;
using HITSBlazor.Components.Tables.TableHeader;
using HITSBlazor.Models.Ideas.Enums;
using HITSBlazor.Models.Teams.Entities;
using HITSBlazor.Models.Users.Entities;
using HITSBlazor.Models.Users.Enums;
using HITSBlazor.Services;
using HITSBlazor.Services.Modal;
using HITSBlazor.Services.Teams;
using HITSBlazor.Services.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;

namespace HITSBlazor.Pages.Admin.AllUsers
{
    [Authorize]
    [Route("/admin/users")]
    public partial class AllUsers
    {
        [Inject]
        private IUserService UserService { get; set; } = null!;

        [Inject]
        private ModalService ModalService { get; set; } = null!;

        private string SeacrhText { get; set; } = string.Empty;
        private string? _orderBy = null;
        private bool? _byDescending = null;
        private HashSet<RoleType> SelectedUserRoles { get; set; } = [];
        private bool? InTeam { get; set; }

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

        private List<User> _users = [];

        protected override async Task OnInitializedAsync()
        {
            await LoadUsersAsync();
        }

        private async Task LoadUsersAsync()
        {
            _users = await UserService.GetUsersAsync(
                searchText: SeacrhText,
                orderBy: _orderBy,
                byDescending: _byDescending,
                selectedRoles: SelectedUserRoles
            );
            await InvokeAsync(StateHasChanged);
        }

        private async Task SearchUser(string searchText)
        {
            SeacrhText = searchText;
            await LoadUsersAsync();
        }

        private async Task SortUser(string? value, bool? state)
        {
            _orderBy = value;
            _byDescending = state;
            await LoadUsersAsync();
        }

        private async Task OnRoleTypeChanged(RoleType role, bool isChecked)
        {
            if (isChecked)
                SelectedUserRoles.Add(role);
            else
                SelectedUserRoles.Remove(role);

            await LoadUsersAsync();
        }

        private async Task ResetFilters()
        {
            SelectedUserRoles.Clear();
            InTeam = null;
            await LoadUsersAsync();
        }

        private void ShowUserProfile(Guid userId) => ModalService.ShowProfileModal(userId);

        private async Task OnUserAction(TableActionContext context)
        {
            if (context.Action == MenuAction.ViewProfile)
            {
                if (context.Item is Guid guid)
                    ShowUserProfile(guid);
            }
            else if (context.Action == MenuAction.Edit)
            {
                if (context.Item is Guid guid)
                    Console.WriteLine("редактор пользователя");
            }
            else if (context.Action == MenuAction.Delete)
            {
                if (context.Item is not User user || !await UserService.DeleteUserAsync(user))
                    return;

                _users.Remove(user);
            }
        }
    }
}
