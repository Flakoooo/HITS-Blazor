using HITSBlazor.Models.Users.Entities;
using HITSBlazor.Models.Users.Enums;

namespace HITSBlazor.Models.Common.Entities
{
    public class UsersGroup
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public List<User> Members { get; set; } = [];
        public List<RoleType> Roles { get; set; } = [];

        public UsersGroup() { }

        public UsersGroup(UsersGroup other)
        {
            Id = other.Id;
            Name = other.Name;
            Roles = other.Roles;
            Members = other.Members;
        }

        public UsersGroup WithUsers(List<User> members)
        {
            var copy = new UsersGroup(this)
            {
                Members = members
            };
            return copy;
        }
    }
}
