namespace HITSBlazor.Models.Auth.Response
{
    public class RegisterResponse
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; } = string.Empty;

        private RegisterResponse() { }

        public static RegisterResponse Success(string message) => new()
        {
            IsSuccess = true,
            Message = message
        };

        public static RegisterResponse Failure(string message) => new()
        {
            IsSuccess = false,
            Message = message
        };
    }
}
