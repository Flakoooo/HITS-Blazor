using HITSBlazor.Models.Common.Responses;
using HITSBlazor.Models.Users.Entities;
using HITSBlazor.Models.Users.Enums;
using HITSBlazor.Models.Users.Requests;

namespace HITSBlazor.Services.Users
{
    public class UserService(
        UserApi userApi, 
        ILogger<UserService> logger, 
        GlobalNotificationService globalNotificationService
    ) : IUserService
    {
        private readonly UserApi _userApi = userApi;
        private readonly ILogger<UserService> _logger = logger;
        private readonly GlobalNotificationService _globalNotificationService = globalNotificationService;

        public event Action<User>? OnUserHasUpdated;
        public event Action<User>? OnUserHasDeleted;

        //TODOO: возможно добавить получение одного пользователя

        public async Task<ListDataResponse<User>> GetUsersAsync(
            int page,
            string? searchText,
            string? orderBy,
            bool? byDescending,
            bool? inTeam,
            Guid? ignoredTeam,
            IEnumerable<RoleType>? selectedRoles,
            IEnumerable<Guid>? ignoredIds
        )
        {
            var result = await _userApi.GetUsersAsync(
                page,
                searchText: searchText,
                orderBy: orderBy,
                byDescending: byDescending,
                inTeam: inTeam,
                ignoredTeam: ignoredTeam,
                selectedRoles: selectedRoles,
                ignoredIds: ignoredIds
            );

            if (result.IsSuccess && result.Response is not null)
                return result.Response;

            if (!string.IsNullOrWhiteSpace(result.Message))
            {
                _globalNotificationService.ShowError("Не удалось получить список пользователей");
                if (_logger.IsEnabled(LogLevel.Warning))
                    _logger.LogWarning("Get users failed: {Error}", result.Message);
            }

            return new ListDataResponse<User>(0, []);
        }

        public async Task<bool> UpdateUser(UpdateUserRequest request)
        {
            var result = await _userApi.UpdateUserAsync(request);

            if (result.IsSuccess && result.Response is not null)
            {
                _globalNotificationService.ShowError(result.Response);
                return true;
            }

            if (!string.IsNullOrWhiteSpace(result.Message))
            {
                _globalNotificationService.ShowError("Не удалось обновить пользователя");
                if (_logger.IsEnabled(LogLevel.Warning))
                    _logger.LogWarning("Update user failed: {Error}", result.Message);
            }

            return false;
        }

        public async Task<bool> DeleteUserAsync(User user)
        {
            var result = await _userApi.DeleteUserAsync(user.Id);

            if (result.IsSuccess && result.Response is not null)
            {
                _globalNotificationService.ShowError(result.Response);
                return true;
            }

            if (!string.IsNullOrWhiteSpace(result.Message))
            {
                _globalNotificationService.ShowError(result.Message);
                if (_logger.IsEnabled(LogLevel.Warning))
                    _logger.LogWarning("Update user failed: {Error}", result.Message);
            }

            return false;
        }
    }
}
