using HITSBlazor.Models.Common.Entities;
using HITSBlazor.Models.Users.Entities;
using HITSBlazor.Models.Users.Enums;

namespace HITSBlazor.Services.UsersGroups
{
    public interface IUsersGroupsService
    {
        event Func<Task>? OnUsersGroupsStateChanged;
        event Action? OnUsersGroupsStateUpdated;

        Task<List<UsersGroup>> GetUsersGroupsAsync(string? searchText = null, HashSet<RoleType>? selectedRoles = null);
        Task<UsersGroup?> GetUsersGroupByIdAsync(Guid usersGroupId);
        Task<bool> CreateUsersGroup(string name, List<User> members, List<RoleType> roles);
        Task<bool> UpdateUsersGroup(Guid usersGroupId, string name, List<User> members, List<RoleType> roles);
        Task<bool> DeleteUsersGroupsAsync(UsersGroup usersGroup);
    }
}
