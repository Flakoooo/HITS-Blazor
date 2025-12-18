namespace HITSBlazor.Models.Auth.Response
{
    public class ResetPasswordResponse
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; } = string.Empty;

        private ResetPasswordResponse() { }

        public static ResetPasswordResponse Success(string message) => new()
        {
            IsSuccess = true,
            Message = message
        };

        public static ResetPasswordResponse Failure(string message) => new()
        {
            IsSuccess = false,
            Message = message
        };
    }
}
