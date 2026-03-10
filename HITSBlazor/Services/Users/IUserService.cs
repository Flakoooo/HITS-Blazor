using HITSBlazor.Models.Users.Entities;
using HITSBlazor.Models.Users.Enums;

namespace HITSBlazor.Services.Users
{
    public interface IUserService
    {
        Task<List<User>> GetUsersAsync(
            string? searchText = null,
            string? orderBy = null,
            bool? byDescending = null,
            HashSet<RoleType>? selectedRoles = null
        );

        Task<bool> DeleteUserAsync(User user);
    }
}
