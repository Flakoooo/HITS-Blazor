using HITSBlazor.Models.Users.Entities;
using HITSBlazor.Models.Users.Enums;

namespace HITSBlazor.Models.Common.Entities
{
    public class UsersGroup
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public List<User> Users { get; set; } = [];
        public List<RoleType> Roles { get; set; } = [];

        public UsersGroup() { }

        public UsersGroup(UsersGroup other)
        {
            Id = other.Id;
            Name = other.Name;
            Roles = other.Roles;
            Users = other.Users;
        }

        public UsersGroup WithUsers(List<User> users)
        {
            var copy = new UsersGroup(this)
            {
                Users = users
            };
            return copy;
        }
    }
}
