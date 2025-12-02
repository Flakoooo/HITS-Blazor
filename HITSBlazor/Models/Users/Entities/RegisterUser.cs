using HITSBlazor.Models.Users.Enums;

namespace HITSBlazor.Models.Users.Entities
{
    public class RegisterUser
    {
        public string Email { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public List<RoleType> Roles { get; set; } = new();
        public string Telephone { get; set; } = string.Empty;
        public string StudyGroup { get; set; } = string.Empty;
    }
}
