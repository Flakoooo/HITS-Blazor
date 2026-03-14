using HITSBlazor.Models.Common.Entities;
using HITSBlazor.Models.Users.Entities;
using HITSBlazor.Models.Users.Enums;
using HITSBlazor.Utils.Mocks.Common;

namespace HITSBlazor.Services.UsersGroups
{
    public class MockUsersGroupsService(GlobalNotificationService globalNotificationService) : IUsersGroupsService
    {
        private readonly GlobalNotificationService _globalNotificationService = globalNotificationService;

        public event Action? OnUsersGroupsStateChanged;

        private List<UsersGroup> _cachedUsersGroups = [];
        private DateTime _lastRefreshTime;
        private readonly TimeSpan _cacheLifetime = TimeSpan.FromMinutes(5);

        private async Task RefreshCacheAsync()
        {
            _cachedUsersGroups = MockUsersGroups.GetAllGroups();
            _lastRefreshTime = DateTime.UtcNow;
        }

        public async Task<List<UsersGroup>> GetUsersGroupsAsync(string? searchText, HashSet<RoleType>? selectedRoles = null)
        {
            if (_cachedUsersGroups.Count == 0 || DateTime.UtcNow - _lastRefreshTime > _cacheLifetime)
                await RefreshCacheAsync();

            var query = _cachedUsersGroups.AsEnumerable();

            if (selectedRoles?.Count > 0)
                query = query.Where(ug => ug.Roles.Any(selectedRoles.Contains));

            if (!string.IsNullOrWhiteSpace(searchText))
                query = query.Where(ug => ug.Name.Contains(searchText, StringComparison.CurrentCultureIgnoreCase));

            return [.. query];
        }

        public async Task<UsersGroup?> GetUsersGroupByIdAsync(Guid usersGroupId)
        {
            var usersGroup = MockUsersGroups.GetGroupById(usersGroupId);
            if (usersGroup is null)
                _globalNotificationService.ShowError("Группа не найдена");

            return usersGroup;
        }

        public async Task<bool> CreateUsersGroup(string name, List<User> members, List<RoleType> roles)
        {
            var usersGroup = MockUsersGroups.CreateUsersGroup(name, [.. members.Select(u => u.Id)], roles);
            if (usersGroup is null)
            {
                _globalNotificationService.ShowError("Не удалось создать группу");
                return false;
            }

            _globalNotificationService.ShowSuccess("Группа успешно создана");
            _cachedUsersGroups.Add(usersGroup);
            OnUsersGroupsStateChanged?.Invoke();
            return true;
        }

        public async Task<bool> UpdateUsersGroup(Guid usersGroupId, string name, List<User> members, List<RoleType> roles)
        {
            var usersGroup = MockUsersGroups.UpdateUsersGroup(usersGroupId, name, [.. members.Select(u => u.Id)], roles);
            if (usersGroup is null)
            {
                _globalNotificationService.ShowError("Не удалось обновить группу");
                return false;
            }

            var usersGroupForUpdate = _cachedUsersGroups.FirstOrDefault(ug => ug.Id == usersGroupId);
            if (usersGroupForUpdate is not null)
            {
                usersGroupForUpdate.Name = name;
                usersGroupForUpdate.Members = [.. members];
                usersGroupForUpdate.Roles = [.. roles];
            }

            _globalNotificationService.ShowSuccess("Группа успешно обновлена");
            OnUsersGroupsStateChanged?.Invoke();
            return true;
        }

        public async Task<bool> DeleteUsersGroupsAsync(UsersGroup usersGroup)
        {
            if (!MockUsersGroups.DeleteUsersGroup(usersGroup))
            {
                _globalNotificationService.ShowError("Не удалось удалить группу");
                return false;
            }

            _cachedUsersGroups.Remove(usersGroup);
            return true;
        }
    }
}
