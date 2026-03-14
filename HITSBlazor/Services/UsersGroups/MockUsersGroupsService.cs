using HITSBlazor.Models.Common.Entities;
using HITSBlazor.Models.Users.Enums;
using HITSBlazor.Utils.Mocks.Common;

namespace HITSBlazor.Services.Groups
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
                query = query.Where(g => g.Roles.Any(selectedRoles.Contains));

            if (!string.IsNullOrWhiteSpace(searchText))
                query = query.Where(g => g.Name.Contains(searchText, StringComparison.CurrentCultureIgnoreCase));

            return [.. query];
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
