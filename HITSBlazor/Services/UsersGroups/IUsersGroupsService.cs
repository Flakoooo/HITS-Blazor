using HITSBlazor.Models.Common.Entities;
using HITSBlazor.Models.Users.Enums;

namespace HITSBlazor.Services.Groups
{
    public interface IUsersGroupsService
    {
        event Action? OnUsersGroupsStateChanged;

        Task<List<UsersGroup>> GetUsersGroupsAsync(string? searchText = null, HashSet<RoleType>? selectedRoles = null);
        Task<bool> DeleteUsersGroupsAsync(UsersGroup usersGroup);
    }
}
