using HITSBlazor.Models.Users.Entities;
using HITSBlazor.Models.Users.Enums;

namespace HITSBlazor.Models.Common.Entities
{
    public class UsersGroup
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public List<User> Users { get; set; } = new();
        public List<RoleType> Roles { get; set; } = new();
    }
}
