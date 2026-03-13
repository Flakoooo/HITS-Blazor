using HITSBlazor.Models.Users.Entities;
using HITSBlazor.Models.Users.Enums;
using HITSBlazor.Models.Users.Requests;
using HITSBlazor.Utils.Mocks.Users;

namespace HITSBlazor.Services.Users
{
    public class MockUserService(GlobalNotificationService globalNotificationService) : IUserService
    {
        private GlobalNotificationService _globalNotificationService = globalNotificationService;

        private List<User> _cachedUsers = [];
        private DateTime _lastRefreshTime;
        private readonly TimeSpan _cacheLifetime = TimeSpan.FromMinutes(5);

        public event Action? OnUsersStateChanged;

        private async Task RefreshCacheAsync()
        {
            _cachedUsers = MockUsers.GetAllUsers();
            _lastRefreshTime = DateTime.UtcNow;
        }

        public async Task<List<User>> GetUsersAsync(
            string? searchText,
            string? orderBy,
            bool? byDescending,
            HashSet<RoleType>? selectedRoles
        )
        {
            if (_cachedUsers.Count == 0 || DateTime.UtcNow - _lastRefreshTime > _cacheLifetime)
                await RefreshCacheAsync();

            var query = _cachedUsers.AsEnumerable();

            if (selectedRoles?.Count > 0)
                query = query.Where(u => u.Roles.Any(selectedRoles.Contains));

            if (!string.IsNullOrWhiteSpace(searchText))
                query = query.Where(u => u.FullName.Contains(searchText, StringComparison.CurrentCultureIgnoreCase));

            if (!string.IsNullOrWhiteSpace(orderBy) && byDescending.HasValue)
            {
                query = (orderBy, byDescending.Value) switch
                {
                    (nameof(User.CreatedAt), true) => query.OrderByDescending(u => u.CreatedAt),
                    (nameof(User.CreatedAt), false) => query.OrderBy(u => u.CreatedAt),
                    _ => query
                };
            }

            return [.. query];
        }

        public async Task<bool> UpdateUser(UpdateUserRequest request)
        {
            if (!MockUsers.UpdateUser(request))
            {
                _globalNotificationService.ShowError("Не удалось обновить пользователя");
                return false;
            }

            var userForUpdate = _cachedUsers.FirstOrDefault(u => u.Id == request.Id);
            if (userForUpdate is not null)
            {
                userForUpdate.Email = request.Email;
                userForUpdate.FirstName = request.FirstName;
                userForUpdate.LastName = request.LastName;
                userForUpdate.Telephone = request.Telephone;
                userForUpdate.StudyGroup = request.StudyGroup;
            }

            OnUsersStateChanged?.Invoke();
            return true;
        }

        public async Task<bool> DeleteUserAsync(User user)
        {
            if (!MockUsers.DeleteUser(user))
            {
                _globalNotificationService.ShowError("Не удалось удалить пользователя");
                return false;
            }

            _cachedUsers.Remove(user);
            return true;
        }
    }
}
