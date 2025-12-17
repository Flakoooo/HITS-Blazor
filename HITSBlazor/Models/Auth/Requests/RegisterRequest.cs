namespace HITSBlazor.Models.Auth.Requests
{
    public class RegisterRequest
    {
        public string Email { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Telephone { get; set; } = string.Empty;
        public string StudyGroup { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}
