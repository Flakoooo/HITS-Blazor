using HITSBlazor.Components.ActionMenus.BaseActionMenu;
using HITSBlazor.Components.Tables.TableComponent;
using HITSBlazor.Components.Tables.TableHeader;
using HITSBlazor.Models.Users.Entities;
using HITSBlazor.Services.Modal;
using HITSBlazor.Services.Teams;
using HITSBlazor.Services.Users;
using Microsoft.AspNetCore.Components;

namespace HITSBlazor.Components.Modals.CenterModals.AddTeamMembersModal
{
    public partial class AddTeamMembersModal
    {
        [Inject]
        private ITeamService TeamService { get; set; } = null!;

        [Inject]
        private IUserService UserService { get; set; } = null!;

        [Inject]
        private ModalService ModalService { get; set; } = null!;

        private bool _isLoading = true;

        private TableComponent? _tableComponent;

        private string SeacrhText { get; set; } = string.Empty;
        private readonly List<User> _users = [];

        private string SearchSkillText { get; set; } = string.Empty;
        private HashSet<Guid> SelectedSkillIds { get; set; } = [];

        private readonly HashSet<User> _selectedUsers = [];

        private readonly List<TableHeaderItem> _tableHeader = 
        [
            new() { Text = "Почта",     ColumnClass = "col-4" },
            new() { Text = "Имя",       ColumnClass = "col-4" },
            new() { Text = "Фамилия",   ColumnClass = "col-6" }
        ];

        protected override async Task OnInitializedAsync()
        {
            _isLoading = true;

            await LoadUsersAsync();

            _isLoading = false;
            MarkAsInitialized();
        }

        protected override async Task OnLoadMoreItemsAsync()
            => await LoadUsersAsync(append: true);

        protected override async Task AdditionalAfterRenderMethod()
        {
            if (_tableComponent != null)
                _tableContainer = _tableComponent.ScrollContainer;
        }

        protected override int GetCurrentItemsCount() => _users.Count;

        private async Task LoadUsersAsync(bool append = false)
        {
            await LoadDataAsync(
                _users,
                () => UserService.GetUsersAsync(
                    _currentPage,
                    searchText: SeacrhText
                ),
                append: append
            );
        }

        private async Task SearchUser(string value)
        {
            SeacrhText = value;
            ResetPagination();
            await LoadUsersAsync();
        }

        private async Task FiltersHasUpdated()
        {
            ResetPagination();
            await LoadUsersAsync();
        }

        private async Task ResetFilters()
        {
            SearchSkillText = string.Empty;
            SelectedSkillIds = [];
            ResetPagination();
            await LoadUsersAsync();
        }

        private async Task ConfirmUsers()
        {
            TeamService.InvokeInvitationEvent(_selectedUsers);
            await ModalService.Close(ModalType.Center);
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
