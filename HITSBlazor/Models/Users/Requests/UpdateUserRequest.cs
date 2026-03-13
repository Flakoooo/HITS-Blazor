using HITSBlazor.Models.Users.Enums;

namespace HITSBlazor.Models.Users.Requests
{
    public class UpdateUserRequest
    {
        public Guid Id { get; set; }
        public string Email { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public List<RoleType> Roles { get; set; } = [];
        public string Telephone { get; set; } = string.Empty;
        public string StudyGroup { get; set; } = string.Empty;

    }
}
