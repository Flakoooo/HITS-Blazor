using HITSBlazor.Models.Common.Entities;
using HITSBlazor.Models.Common.Responses;
using HITSBlazor.Models.Users.Entities;
using HITSBlazor.Models.Users.Enums;

namespace HITSBlazor.Services.UsersGroups
{
    public interface IUsersGroupsService
    {
        event Action<UsersGroup>? OnUsersGroupHasCreated;
        event Action<UsersGroup>? OnUsersGroupHasUpdated;
        event Action<UsersGroup>? OnUsersGroupHasDeleted;

        Task<ListDataResponse<UsersGroup>> GetUsersGroupsAsync(
            int page, string? searchText = null, IEnumerable<RoleType>? selectedRoles = null
        );
        Task<UsersGroup?> GetUsersGroupByIdAsync(Guid usersGroupId);
        Task<ListDataResponse<User>> GetUsersGroupMembersAsync(
            Guid usersGroupId,
            int page,
            string? searchText = null
        );
        Task<bool> CreateUsersGroup(string name, IEnumerable<User> members, IEnumerable<RoleType> roles);
        Task<bool> UpdateUsersGroup(
            Guid usersGroupId, 
            string? name = null,
            IEnumerable<Guid>? newMembersIds = null,
            IEnumerable<Guid>? removeMembersIds = null,
            IEnumerable<RoleType>? roles = null
        );
        Task<bool> DeleteUsersGroupsAsync(UsersGroup usersGroup);
    }
}
