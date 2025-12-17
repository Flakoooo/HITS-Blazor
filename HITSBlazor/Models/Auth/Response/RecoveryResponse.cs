namespace HITSBlazor.Models.Auth.Response
{
    public class RecoveryResponse
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; } = string.Empty;

        private RecoveryResponse() { }

        public static RecoveryResponse Success(string message) => new()
        {
            IsSuccess = true,
            Message = message
        };

        public static RecoveryResponse Failure(string message) => new()
        {
            IsSuccess = false,
            Message = message
        };
    }
}
