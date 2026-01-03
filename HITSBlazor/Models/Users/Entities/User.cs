using HITSBlazor.Models.Users.Enums;

namespace HITSBlazor.Models.Users.Entities
{
    public class User
    {
        public Guid Id { get; set; }
        public string Email { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public DateTime? CreatedAt { get; set; }
        public List<RoleType> Roles { get; set; } = [];
        public string Telephone { get; set; } = string.Empty;
        public string StudyGroup { get; set; } = string.Empty;

        public RoleType? Role { get; set; }
    }
}
