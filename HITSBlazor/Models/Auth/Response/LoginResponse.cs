using HITSBlazor.Models.Users.Entities;

namespace HITSBlazor.Models.Auth.Response
{
    public class LoginResponse
    {
        public bool Success { get; set; }
        public string? Token { get; set; }
        public string? ErrorMessage { get; set; }
        public User? User { get; set; }
    }
}
