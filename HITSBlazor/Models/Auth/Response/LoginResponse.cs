namespace HITSBlazor.Models.Auth.Response
{
    public class LoginResponse
    {
        public bool IsSuccess { get; set; }
        public string? ErrorMessage { get; set; }

        private LoginResponse() { }

        public static LoginResponse Success() => new()
        {
            IsSuccess = true
        };

        public static LoginResponse Failure(string? errorMessage) => new()
        {
            IsSuccess = false,
            ErrorMessage = errorMessage
        };
    }
}
