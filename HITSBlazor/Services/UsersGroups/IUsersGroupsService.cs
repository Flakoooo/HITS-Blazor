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
        Task<bool> CreateUsersGroup(string name, HashSet<User> members, IEnumerable<RoleType> roles);
        Task<bool> UpdateUsersGroup(Guid usersGroupId, string name, HashSet<User> members, IEnumerable<RoleType> roles);
        Task<bool> DeleteUsersGroupsAsync(UsersGroup usersGroup);
    }
}
