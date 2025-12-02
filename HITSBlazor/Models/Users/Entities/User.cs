using HITSBlazor.Models.Users.Enums;

namespace HITSBlazor.Models.Users.Entities
{
    public class User
    {
        public string Id { get; set; } = string.Empty;
        public string? Token { get; set; }
        public string Email { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string? CreatedAt { get; set; }
        public List<RoleType> Roles { get; set; } = new();
        public RoleType? Role { get; set; }
        public DateTime? LastLogin { get; set; }
        public string Telephone { get; set; } = string.Empty;
        public string StudyGroup { get; set; } = string.Empty;
    }
}
