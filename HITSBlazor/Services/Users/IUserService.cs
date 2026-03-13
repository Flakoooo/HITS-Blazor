using HITSBlazor.Models.Users.Entities;
using HITSBlazor.Models.Users.Enums;
using HITSBlazor.Models.Users.Requests;

namespace HITSBlazor.Services.Users
{
    public interface IUserService
    {
        event Action? OnUsersStateChanged;

        Task<List<User>> GetUsersAsync(
            string? searchText = null,
            string? orderBy = null,
            bool? byDescending = null,
            HashSet<RoleType>? selectedRoles = null
        );

        Task<bool> UpdateUser(UpdateUserRequest request);

        Task<bool> DeleteUserAsync(User user);
    }
}
