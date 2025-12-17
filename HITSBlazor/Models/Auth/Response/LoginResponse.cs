using HITSBlazor.Models.Users.Entities;

namespace HITSBlazor.Models.Auth.Response
{
    public class LoginResponse
    {
        public bool IsSuccess { get; set; }
        public string? Token { get; set; }
        public string? ErrorMessage { get; set; }
        public User? User { get; set; }

        private LoginResponse() { }

        public static LoginResponse Success(string? token, User? user) => new()
        {
            IsSuccess = true,
            Token = token,
            User = user
        };

        public static LoginResponse Failure(string? errorMessage) => new()
        {
            IsSuccess = false,
            ErrorMessage = errorMessage
        };
    }
}
