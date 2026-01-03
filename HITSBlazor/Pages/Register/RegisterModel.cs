using System.Text.Json.Serialization;

namespace HITSBlazor.Pages.Register
{
    public class RegisterModel
    {
        public string Email { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Telephone { get; set; } = string.Empty;
        public string StudyGroup { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}
