using HITSBlazor.Models.Users.Entities;
using HITSBlazor.Models.Users.Enums;
using HITSBlazor.Services;
using HITSBlazor.Services.Modal;
using HITSBlazor.Services.Users;
using HITSBlazor.Services.UsersGroups;
using HITSBlazor.Utils.Models;
using Microsoft.AspNetCore.Components;
using System.Data;

namespace HITSBlazor.Components.Modals.CenterModals.UsersGroupModal
{
    public partial class UsersGroupModal
    {
        [Inject]
        private IUsersGroupsService UsersGroupsService { get; set; } = null!;

        [Inject]
        private IUserService UserService { get; set; } = null!;

        [Inject]
        private ModalService ModalService { get; set; } = null!;

        [Inject]
        private GlobalNotificationService NotificationService { get; set; } = null!;

        [Parameter]
        public Guid? UsersGroupId { get; set; } = null!;

        private bool _isLoading = true;
        private bool _submitting = false;

        private string UsersGroupName { get; set; } = string.Empty;

        private List<User> GroupMembers { get; set; } = [];
        private HashSet<User> MembersForAdd { get; set; } = [];
        private HashSet<User> MembersForRemove { get; set; } = [];

        private List<EnumViewModel<RoleType>> AllRoles { get; set; } = [];

        private HashSet<EnumViewModel<RoleType>> SelectedRoles { get; set; } = [];

        protected override async Task OnInitializedAsync()
        {
            _isLoading = true;

            if (UsersGroupId.HasValue)
            {
                var usersGroup = await UsersGroupsService.GetUsersGroupByIdAsync(UsersGroupId.Value);
                UsersGroupName = usersGroup?.Name ?? string.Empty;

                foreach (var role in usersGroup?.Roles ?? [])
                    SelectedRoles.Add(new(role));
            }

            foreach (var role in Enum.GetValues<RoleType>())
                AllRoles.Add(new(role));

            _isLoading = false;
        }

        private void AddUser(User user)
        {
            if (UsersGroupId.HasValue)
                if (!MembersForRemove.Remove(user))
                    MembersForAdd.Add(user);

            GroupMembers.Add(user);
        }

        private void RemoveUser(User user)
        {
            if (UsersGroupId.HasValue)
                if (!MembersForAdd.Remove(user))
                    MembersForRemove.Add(user);

            GroupMembers.Remove(user);
        }

        private bool CheckValidValues()
        {
            if (string.IsNullOrWhiteSpace(UsersGroupName)) return false;
            if (GroupMembers.Count == 0) return false;
            if (SelectedRoles.Count == 0) return false;

            return true;
        }

        private async Task SendUsersGroup()
        {
            _submitting = true;

            if (!CheckValidValues())
            {
                NotificationService.ShowError("Заполнены не все поля");
                _submitting = false;
                return;
            }

            bool result;
            if (UsersGroupId.HasValue)
            {
                result = await UsersGroupsService.UpdateUsersGroup(
                    UsersGroupId.Value,
                    UsersGroupName,
                    MembersForAdd.Select(u => u.Id),
                    MembersForRemove.Select(u => u.Id),
                    SelectedRoles.Select(r => r.Value)
                );
            }
            else
            {
                result = await UsersGroupsService.CreateUsersGroup(
                    UsersGroupName,
                    GroupMembers,
                    SelectedRoles.Select(r => r.Value)
                );
            }

            if (result)
                await ModalService.Close(ModalType.Center);

            _submitting = false;
        }
    }
}
