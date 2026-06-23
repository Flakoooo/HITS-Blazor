using HITSBlazor.Models.Common.Entities;
using HITSBlazor.Models.Common.Responses;
using HITSBlazor.Models.Users.Entities;
using HITSBlazor.Models.Users.Enums;
using HITSBlazor.Services.Companies;
using HITSBlazor.Utils.Mocks.Common;
using System.ComponentModel.Design;

namespace HITSBlazor.Services.UsersGroups
{
    public class UsersGroupsService(
        UsersGroupApi usersGroupApi,
        ILogger<UsersGroupsService> logger,
        GlobalNotificationService globalNotificationService
    ) : IUsersGroupsService
    {
        private readonly UsersGroupApi _usersGroupApi = usersGroupApi;
        private readonly ILogger<UsersGroupsService> _logger = logger;
        private readonly GlobalNotificationService _globalNotificationService = globalNotificationService;

        public event Action<UsersGroup>? OnUsersGroupHasCreated;
        public event Action<UsersGroup>? OnUsersGroupHasUpdated;
        public event Action<UsersGroup>? OnUsersGroupHasDeleted;

        public async Task<ListDataResponse<UsersGroup>> GetUsersGroupsAsync(
            int page,
            string? searchText, 
            IEnumerable<RoleType>? selectedRoles
        )
        {
            var result = await _usersGroupApi.GetGroupsAsync(
                page, searchText: searchText, selectedRoles: selectedRoles
            );

            if (result.IsSuccess && result.Response is not null)
                return result.Response;

            if (!string.IsNullOrWhiteSpace(result.Message))
            {
                _globalNotificationService.ShowError(result.Message);
                if (_logger.IsEnabled(LogLevel.Warning))
                    _logger.LogWarning("Get groups failed: {Error}", result.Message);
            }

            return new ListDataResponse<UsersGroup>(0, []);
        }

        public async Task<UsersGroup?> GetUsersGroupByIdAsync(Guid usersGroupId)
        {
            var result = await _usersGroupApi.GetGroupAsync(usersGroupId);

            if (!string.IsNullOrWhiteSpace(result.Message))
            {
                _globalNotificationService.ShowError(result.Message);
                if (_logger.IsEnabled(LogLevel.Warning))
                    _logger.LogWarning("Get group failed: {Error}", result.Message);
            }

            return result.Response;
        }

        public async Task<ListDataResponse<User>> GetUsersGroupMembersAsync(
            Guid usersGroupId,
            int page,
            string? searchText
        )
        {
            var result = await _usersGroupApi.GetGroupMembersAsync(
                usersGroupId, page, searchText: searchText
            );

            if (result.IsSuccess && result.Response is not null)
                return result.Response;

            if (!string.IsNullOrWhiteSpace(result.Message))
            {
                _globalNotificationService.ShowError(result.Message);
                if (_logger.IsEnabled(LogLevel.Warning))
                    _logger.LogWarning("Get group members failed: {Error}", result.Message);
            }

            return new ListDataResponse<User>(0, []);
        }

        public async Task<bool> CreateUsersGroup(string name, IEnumerable<User> members, IEnumerable<RoleType> roles)
        {
            var result = await _usersGroupApi.CreateGroupAsync(
                name, roles, members
            );

            if (result.IsSuccess && result.Response is not null)
            {
                OnUsersGroupHasCreated?.Invoke(result.Response);
                _globalNotificationService.ShowSuccess("Группа успешно создана!");
                return true;
            }

            if (!string.IsNullOrWhiteSpace(result.Message))
            {
                _globalNotificationService.ShowError(result.Message);
                if (_logger.IsEnabled(LogLevel.Warning))
                    _logger.LogWarning("Create group failed: {Error}", result.Message);
            }

            return false;
        }

        public async Task<bool> UpdateUsersGroup(
            Guid usersGroupId, 
            string? name, 
            IEnumerable<Guid>? newMembersIds,
            IEnumerable<Guid>? removeMembersIds, 
            IEnumerable<RoleType>? roles
        )
        {
            var result = await _usersGroupApi.UpdateGroupAsync(
                usersGroupId, name, roles, newMembersIds, removeMembersIds
            );

            if (result.IsSuccess && result.Response is not null)
            {
                OnUsersGroupHasUpdated?.Invoke(result.Response);
                _globalNotificationService.ShowSuccess("Группа успешно обновлена!");
                return true;
            }

            if (!string.IsNullOrWhiteSpace(result.Message))
            {
                _globalNotificationService.ShowError(result.Message);
                if (_logger.IsEnabled(LogLevel.Warning))
                    _logger.LogWarning("Update group failed: {Error}", result.Message);
            }

            return false;
        }

        public async Task<bool> DeleteUsersGroupsAsync(UsersGroup usersGroup)
        {
            var result = await _usersGroupApi.DeleteGroupAsync(usersGroup.Id);

            if (result.IsSuccess && result.Response is not null)
            {
                OnUsersGroupHasDeleted?.Invoke(usersGroup);
                _globalNotificationService.ShowSuccess("Группа успешно удалена!");
                return true;
            }

            if (!string.IsNullOrWhiteSpace(result.Message))
            {
                _globalNotificationService.ShowError(result.Message);
                if (_logger.IsEnabled(LogLevel.Warning))
                    _logger.LogWarning("Delete group failed: {Error}", result.Message);
            }

            return false;
        }
    }
}
