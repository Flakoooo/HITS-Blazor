using HITSBlazor.Models.Common.Entities;
using HITSBlazor.Models.Common.Responses;
using HITSBlazor.Models.Users.Entities;
using HITSBlazor.Models.Users.Enums;
using HITSBlazor.Utils.Mocks.Common;

namespace HITSBlazor.Services.UsersGroups
{
    public class MockUsersGroupsService(GlobalNotificationService globalNotificationService) : IUsersGroupsService
    {
        private readonly GlobalNotificationService _globalNotificationService = globalNotificationService;

        public event Action<UsersGroup>? OnUsersGroupHasCreated;
        public event Action<UsersGroup>? OnUsersGroupHasUpdated;
        public event Action<UsersGroup>? OnUsersGroupHasDeleted;

        public async Task<ListDataResponse<UsersGroup>> GetUsersGroupsAsync(
            int page,
            string? searchText, 
            IEnumerable<RoleType>? selectedRoles
        ) => MockUsersGroups.GetAllGroups(
            page, searchText: searchText, selectedRoles: selectedRoles?.ToHashSet()
        );

        public async Task<UsersGroup?> GetUsersGroupByIdAsync(Guid usersGroupId)
        {
            var usersGroup = MockUsersGroups.GetGroupById(usersGroupId);
            if (usersGroup is null)
                _globalNotificationService.ShowError("Группа не найдена");

            return usersGroup;
        }

        public async Task<bool> CreateUsersGroup(string name, HashSet<User> members, IEnumerable<RoleType> roles)
        {
            var usersGroup = MockUsersGroups.CreateUsersGroup(name, members.Select(u => u.Id).ToList(), roles.ToList());
            if (usersGroup is null)
            {
                _globalNotificationService.ShowError("Не удалось создать группу");
                return false;
            }

            _globalNotificationService.ShowSuccess("Группа успешно создана");
            OnUsersGroupHasCreated?.Invoke(usersGroup);
            return true;
        }

        public async Task<bool> UpdateUsersGroup(Guid usersGroupId, string name, HashSet<User> members, IEnumerable<RoleType> roles)
        {
            var usersGroup = MockUsersGroups.UpdateUsersGroup(usersGroupId, name, members.Select(u => u.Id).ToList(), roles.ToList());
            if (usersGroup is null)
            {
                _globalNotificationService.ShowError("Не удалось обновить группу");
                return false;
            }

            _globalNotificationService.ShowSuccess("Группа успешно обновлена");
            OnUsersGroupHasUpdated?.Invoke(usersGroup);
            return true;
        }

        public async Task<bool> DeleteUsersGroupsAsync(UsersGroup usersGroup)
        {
            if (!MockUsersGroups.DeleteUsersGroup(usersGroup))
            {
                _globalNotificationService.ShowError("Не удалось удалить группу");
                return false;
            }

            OnUsersGroupHasDeleted?.Invoke(usersGroup);
            return true;
        }
    }
}
