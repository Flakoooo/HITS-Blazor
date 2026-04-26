using HITSBlazor.Models.Common.Responses;
using HITSBlazor.Models.Users.Entities;
using HITSBlazor.Models.Users.Enums;
using HITSBlazor.Models.Users.Requests;
using HITSBlazor.Utils.Mocks.Users;

namespace HITSBlazor.Services.Users
{
    public class MockUserService(GlobalNotificationService globalNotificationService) : IUserService
    {
        private readonly GlobalNotificationService _globalNotificationService = globalNotificationService;

        public event Action<User>? OnUserHasUpdated;
        public event Action<User>? OnUserHasDeleted;

        public async Task<ListDataResponse<User>> GetUsersAsync(
            int page,
            string? searchText,
            string? orderBy,
            bool? byDescending,
            HashSet<RoleType>? selectedRoles
        ) => MockUsers.GetAllUsers(
            page,
            searchText: searchText,
            orderBy: orderBy,
            byDescending: byDescending,
            selectedRoles: selectedRoles
        );

        //TODO: событие с изменение пользователя
        public async Task<bool> UpdateUser(UpdateUserRequest request)
        {
            var newUser = MockUsers.UpdateUser(request);
            if (newUser is null)
            {
                _globalNotificationService.ShowError("Не удалось обновить пользователя");
                return false;
            }

            OnUserHasUpdated?.Invoke(newUser);
            return true;
        }

        //TODO: событие с удалением пользователя
        public async Task<bool> DeleteUserAsync(User user)
        {
            if (!MockUsers.DeleteUser(user))
            {
                _globalNotificationService.ShowError("Не удалось удалить пользователя");
                return false;
            }

            OnUserHasDeleted?.Invoke(user);
            return true;
        }
    }
}
